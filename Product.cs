using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodStore
{
    class Product : IProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Measure { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
