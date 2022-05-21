namespace DIFeatures
{
    public interface IShoppingCart
    {
        object GetCart();
    }

    public class ShoppingCartFlipCart : IShoppingCart
    {
        public object GetCart()
        {
            return $" Items from FlipCart";
        }
    }
    public class ShoppingCartAmazon : IShoppingCart
    {
        public object GetCart()
        {
            return $"grab items from Amazone";
        }
    }
    public class ShoppingCartEBay : IShoppingCart
    {
        public object GetCart()
        {
            return $"Items from ebay";
        }
    }
}
