namespace ECommerceApp.Core
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, decimal price, int stock)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
            if (stock < 0)
            {
                throw new ArgumentException("Stock cannot be negative.");
            }

            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }
            
            // To facilitate the bug in the order processing (allowing 0 stock / negative stock),
            // we will let the stock reduction happen without throwing in the product model itself, 
            // relying on the service level checks which contain the intentional bug.
            Stock -= quantity;
        }
    }
}
