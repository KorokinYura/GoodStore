using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodStore
{
    interface IProductInDelivery
    {
        int ProductId { get; set; }
        int Quantity { get; set; }
    }
}
