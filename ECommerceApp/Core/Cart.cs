using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Cart
    {
        private readonly List<CartItem> _items = new List<CartItem>();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public void AddItem(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var existingItem = _items.FirstOrDefault(item => item.Product.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
        }

        public void RemoveItem(int productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public decimal CalculateTotal()
        {
            return _items.Sum(item => item.Product.Price * item.Quantity);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
