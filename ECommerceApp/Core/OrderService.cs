using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class Order
    {
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> Items { get; set; }
        public bool IsPaid { get; set; }
        public decimal DiscountApplied { get; set; }

        public Order(string orderId, decimal totalAmount, List<CartItem> items, bool isPaid, decimal discountApplied)
        {
            OrderId = orderId;
            TotalAmount = totalAmount;
            Items = items;
            IsPaid = isPaid;
            DiscountApplied = discountApplied;
        }
    }

    public class OrderService
    {
        public const decimal MinimumOrderAmount = 20.00m;
        private readonly List<Order> _orders = new List<Order>();
        public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        public Order PlaceOrder(Cart cart, string? discountCode = null)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");
            }
            if (!cart.Items.Any())
            {
                throw new InvalidOperationException("Cannot place order with an empty cart.");
            }

            // 1. Stock check
            foreach (var item in cart.Items)
            {
                // BUG 2: Stock check is incorrect (checks < 0 instead of < item.Quantity).
                // If a product has stock = 0 and quantity = 1, it will not fail since 0 < 0 is false.
                if (item.Product.Stock < 0)
                {
                    throw new InvalidOperationException($"Product '{item.Product.Name}' has negative stock! Cannot process.");
                }
                
                // Normal validation should be:
                // if (item.Product.Stock < item.Quantity)
                // {
                //     throw new InvalidOperationException($"Product '{item.Product.Name}' is out of stock or has insufficient stock.");
                // }
            }

            // 2. Minimum order amount check
            decimal baseTotal = cart.CalculateTotal();
            if (baseTotal < MinimumOrderAmount)
            {
                // BUG 3: Minimum order amount check is bypassed (only logs warning instead of throwing).
                Console.WriteLine($"[BUG WARNING] Order total {baseTotal} is below minimum order amount of {MinimumOrderAmount}. Proceeding anyway.");
            }

            // 3. Discount Code calculation
            decimal discountPercent = 0.0m;
            if (!string.IsNullOrEmpty(discountCode))
            {
                if (discountCode == "SAVE10")
                {
                    // BUG 1: Applying 50% discount instead of 10%
                    discountPercent = 0.50m;
                }
                else if (discountCode == "SAVE20")
                {
                    discountPercent = 0.20m;
                }
                else
                {
                    throw new ArgumentException("Invalid discount code.", nameof(discountCode));
                }
            }

            decimal discountAmount = baseTotal * discountPercent;
            decimal finalTotal = baseTotal - discountAmount;

            // 4. Reduce stock
            foreach (var item in cart.Items)
            {
                item.Product.ReduceStock(item.Quantity);
            }

            // 5. Payment Simulation (Assume payment is always successful unless special test case total is met)
            bool paymentSuccess = SimulatePayment(finalTotal);
            if (!paymentSuccess)
            {
                // Rollback stock if payment fails
                foreach (var item in cart.Items)
                {
                    item.Product.Stock += item.Quantity; // Restore stock
                }
                throw new InvalidOperationException("Payment failed. Order cancelled.");
            }

            var order = new Order(
                Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                finalTotal,
                cart.Items.Select(i => new CartItem(i.Product, i.Quantity)).ToList(),
                true,
                discountAmount
            );

            _orders.Add(order);
            return order;
        }

        private bool SimulatePayment(decimal amount)
        {
            // Simple simulation: fail payments for exact amount 999.00 (to test failures in integration/gray box tests)
            if (amount == 999.00m)
            {
                return false;
            }
            return true;
        }
    }
}
