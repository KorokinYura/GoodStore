using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GoodStore
{
    class Repository : IDisposable
    {
        private readonly SqlConnection con;

        public Repository()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            con = new SqlConnection(config["connectionString"]);
        }

        public void Dispose()
        {
            con.Dispose();
        }

        public void AddProduct(IProduct product)
        {
            string sql = @"INSERT INTO Storage (Name, Measure, Price, Quantity)
                        VALUES (@Name, @Measure, @Price, 0)";

            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Measure", product.Measure);
            command.Parameters.AddWithValue("@Price", product.Price);

            con.Open();
            command.ExecuteNonQuery();
        }

        public bool AddInvoice(IDelivery delivery, List<IProductInDelivery> prodInDel)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            try
            {
                string comStr = @"INSERT INTO Delivery (Date, Delivery_Withdraw)
                        VALUES (@Date, @Delivery);
                        SELECT CAST(scope_identity() AS int)";
                SqlCommand command = new SqlCommand(comStr, con, transaction);
                command.Parameters.AddWithValue("@Date", delivery.Date);
                command.Parameters.AddWithValue("@Delivery", delivery.IsDelivery);

                int deliveryId = (int)command.ExecuteScalar();

                foreach (IProductInDelivery prod in prodInDel)
                {
                    comStr = @"INSERT INTO [Product-Delivery] (IdDelivery, IdProduct, Quantity)
                        VALUES (@DeliveryId, @ProdId, @Quantity);
                        UPDATE Storage SET 
                        Quantity = Quantity + @QuantityUpd
                        WHERE IdProduct = @ProdId;";
                    command = new SqlCommand(comStr, con, transaction);
                    command.Parameters.AddWithValue("@ProdId", prod.ProductId);
                    command.Parameters.AddWithValue("@DeliveryId", deliveryId);
                    command.Parameters.AddWithValue("@Quantity", prod.Quantity);
                    command.Parameters.AddWithValue("@QuantityUpd", delivery.IsDelivery ? prod.Quantity : -prod.Quantity);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }

            return true;
        }

        public List<IProduct> GetProducts()
        {
            string commandString = "SELECT * FROM Storage";
            SqlCommand command = new SqlCommand(commandString, con);

            List<IProduct> result = new List<IProduct>();

            con.Open();

            using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    result.Add(new Product
                    {
                        Id = (int)reader["IdProduct"],
                        Name = (string)reader["Name"],
                        Measure = (string)reader["Measure"],
                        Price = (int)reader["Price"],
                        Quantity = (int)reader["Quantity"],
                    });
                }
            }

            return result;
        }
    }
}
