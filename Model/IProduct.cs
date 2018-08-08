namespace GoodStore
{
    interface IProduct
    {
        int Id { set; get; }
        string Name { set; get; }
        string Measure { set; get; }
        int Price { set; get; }
        int Quantity { set; get; }
    }
}
