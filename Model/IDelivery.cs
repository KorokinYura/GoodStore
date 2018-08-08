using System;

namespace GoodStore
{
    interface IDelivery
    {
        DateTime Date { get; set; }
        bool IsDelivery { get; set; }
    }
}
