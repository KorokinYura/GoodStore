using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodStore
{
    interface IDelivery
    {
        DateTime Date { get; set; }
        bool IsDelivery { get; set; }
    }
}
