using System;
using System.Linq;
using System.Collections.Generic;

namespace GoodStore
{
    static class Store
    {
        public static void ShowMenu()
        {
            Console.WriteLine("What whould you like to do?\n\n" +
                "1) Add new type of product\n" +
                "2) Add an invoice\n" +
                "3) Show the rest of the products in stock\n" +
                "0) Exit\n");
        }

        public static void Work()
        {
            ShowMenu();

            string command = string.Empty;

            while (command != "0")
            {
                command = string.Empty;
                command = Console.ReadLine();
                Console.WriteLine();

                switch (command)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        AddInvoice();
                        break;
                    case "3":
                        ShowProducts();
                        break;
                    case "0":
                        break;
                    default:
                        Console.WriteLine("\nEnter correct information\n");
                        break;
                }
            }
        }

        public static void AddProduct()
        {
            Console.WriteLine("Name of the product:");
            string name = Console.ReadLine();
            Console.WriteLine("Measure of the product:");
            string measure = Console.ReadLine();
            int price;

            while (true)
            {
                Console.WriteLine("Price of the product:");
                string priceStr = Console.ReadLine();

                if (!Int32.TryParse(priceStr, out price) || price < 0)
                {
                    Console.WriteLine("Price must be a number between 0 and 2147483647\n");
                    continue;
                }
                else
                    break;
            }

            using (Repository repo = new Repository())
            {
                repo.AddProduct(new Product
                {
                    Name = name,
                    Measure = measure,
                    Price = Convert.ToInt32(price)
                });
            }

            Console.WriteLine("\nProduct has been succesfully added to the DB\n");

            ShowMenu();
        }

        public static void AddInvoice()
        {
            Console.WriteLine("Whould you like to deliver products or to withdraw them?\n0 - deliver / 1 - withdraw");
            string deliver = Console.ReadLine();

            List<IProduct> products;
            IDelivery delivery;
            List<IProductInDelivery> prodInDel = new List<IProductInDelivery>();

            while (true)
                if (deliver == "0")
                {
                    delivery = new Delivery(true);
                    break;
                }
                else if (deliver == "1")
                {
                    delivery = new Delivery(false);
                    break;
                }
                else
                {
                    Console.WriteLine("\nYou entered incorrect information\n");
                    ShowMenu();
                }

            products = GetProducts();

            while (true)
            {
                IProductInDelivery curProdInDel = AddProductToInvoice(delivery, products);

                for (int i = 0; i < prodInDel.Count || prodInDel.Count == 0; i++)
                    if (prodInDel.Count != 0 && prodInDel[i].ProductId == curProdInDel.ProductId)
                        prodInDel[i].Quantity += curProdInDel.Quantity;
                    else
                        prodInDel.Add(curProdInDel);

                Console.WriteLine("\nDo you want to add one more product?\n0 - yes / any other button - no");

                string add = Console.ReadLine();

                if (add == "0")
                    continue;
                else
                    break;
            }

            using (Repository repo = new Repository())
            {
                if (repo.AddInvoice(delivery, prodInDel))
                    Console.WriteLine("\nThe invoice is succesful");
                else
                    Console.WriteLine("\nSomething went wrong during the invoice");
            }

            Console.WriteLine("\nPress \"Enter\" to continue");
            Console.ReadLine();
            ShowMenu();
        }

        private static IProductInDelivery AddProductToInvoice(IDelivery delivery, List<IProduct> products)
        {
            Console.WriteLine("\nChoose the product or enter 0 to end an invoice\n");

            string prodNum = string.Empty;
            while (true)
            {
                DisplayProducts(products);
                prodNum = Console.ReadLine();

                if (!Int32.TryParse(prodNum, out int num) || num < 0 || num > products.Count)
                    Console.WriteLine("Product with this number does not exist or you wrote wrong value");
                else
                    break;
            }

            Console.WriteLine("Enter the quantity of products");

            string prodQuantity = string.Empty;
            if (delivery.IsDelivery)
            {
                while (true)
                {
                    prodQuantity = Console.ReadLine();

                    if (!Int32.TryParse(prodQuantity, out int quantity) || quantity <= 0)
                        Console.WriteLine("Quantity should be a positive number");
                    else
                        break;
                }
            }
            else
            {
                while (true)
                {
                    prodQuantity = Console.ReadLine();

                    if (!Int32.TryParse(prodQuantity, out int quantity) || quantity <= 0 ||
                        quantity > products[Int32.Parse(prodNum) - 1].Quantity)
                        Console.WriteLine("Quantity should be a positive number and be less then the amount in store");
                    else
                        break;
                }
            }

            if(!delivery.IsDelivery)
                products[Int32.Parse(prodNum) - 1].Quantity -= Int32.Parse(prodQuantity);

            return new ProductInDelivery
            {
                ProductId = products[Int32.Parse(prodNum) - 1].Id,
                Quantity = Int32.Parse(prodQuantity)
            };
        }

        public static void ShowProducts()
        {
            List<IProduct> products = new List<IProduct>();

            products = GetProducts();
            DisplayProducts(products);

            Console.WriteLine("\nPress \"Enter\" to continue");
            Console.ReadLine();
            ShowMenu();
        }

        private static List<IProduct> GetProducts()
        {
            List<IProduct> products;

            using (Repository repo = new Repository())
            {
                products = repo.GetProducts();
            }

            return products;
        }

        private static void DisplayProducts(List<IProduct> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {products[i].Name} - {products[i].Quantity} {products[i].Measure}   Price: {products[i].Price}");
            }
        }
    }
}
