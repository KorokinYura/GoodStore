using System;

namespace GoodStore
{
    class Delivery : IDelivery
    {
        public Delivery(bool isDelivery)
        {
            Date = DateTime.Now;
            IsDelivery = isDelivery;
        }

        public DateTime Date { get; set; }
        public bool IsDelivery { get; set; }
    }
}
