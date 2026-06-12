using System;
using ECommerceApp.Core;

namespace ECommerceApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("        E-Commerce System Demo Application        ");
            Console.WriteLine("==================================================");

            // Initialize Services and Products
            var orderService = new OrderService();
            var phone = new Product(1, "Smartphone", 800.00m, 5);
            var headPhones = new Product(2, "Headphones", 15.00m, 10);
            var outOfStockBook = new Product(3, "OOP Book", 45.00m, 0);

            Console.WriteLine("\n[1] Available Products:");
            Console.WriteLine($"- {phone.Name} ($ {phone.Price:F2}) - Stock: {phone.Stock}");
            Console.WriteLine($"- {headPhones.Name} ($ {headPhones.Price:F2}) - Stock: {headPhones.Stock}");
            Console.WriteLine($"- {outOfStockBook.Name} ($ {outOfStockBook.Price:F2}) - Stock: {outOfStockBook.Stock}");

            // --- Case A: Normal purchase flow ---
            Console.WriteLine("\n--- Case A: Normal Purchase Flow ---");
            var cartA = new Cart();
            cartA.AddItem(phone, 1);
            Console.WriteLine($"Cart A Total: $ {cartA.CalculateTotal():F2}");
            try
            {
                var order = orderService.PlaceOrder(cartA);
                Console.WriteLine($"SUCCESS: Order {order.OrderId} placed. Paid Amount: $ {order.TotalAmount:F2}");
                Console.WriteLine($"Remaining Stock for {phone.Name}: {phone.Stock}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.Message}");
            }

            // --- Case B: Discount application (SAVE10) ---
            Console.WriteLine("\n--- Case B: Discount Application (SAVE10) ---");
            Console.WriteLine("Applying SAVE10. Expected total: $ 800 - 10% = $ 720.");
            var cartB = new Cart();
            var phone2 = new Product(4, "Smartphone 2", 800.00m, 5);
            cartB.AddItem(phone2, 1);
            try
            {
                var order = orderService.PlaceOrder(cartB, "SAVE10");
                Console.WriteLine($"SUCCESS: Order {order.OrderId} placed.");
                Console.WriteLine($"Applied Discount: $ {order.DiscountApplied:F2} (Expected: $ 80.00)");
                Console.WriteLine($"Paid Amount: $ {order.TotalAmount:F2} (Expected: $ 720.00)");
                Console.WriteLine("[BUG DETECTED] Discount was 50% instead of 10%!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.Message}");
            }

            // --- Case C: Stock check bug (0 stock allowed) ---
            Console.WriteLine("\n--- Case C: Stock Control Bug (Product with 0 stock) ---");
            Console.WriteLine($"Attempting to order '{outOfStockBook.Name}' which has Stock: {outOfStockBook.Stock}");
            var cartC = new Cart();
            cartC.AddItem(outOfStockBook, 1);
            try
            {
                var order = orderService.PlaceOrder(cartC);
                Console.WriteLine($"[BUG DETECTED] SUCCESS: Order {order.OrderId} placed for out of stock item!");
                Console.WriteLine($"Remaining Stock for {outOfStockBook.Name}: {outOfStockBook.Stock}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
            }

            // --- Case D: Minimum order amount check bug ($15.00 < $20.00) ---
            Console.WriteLine("\n--- Case D: Minimum Order Amount Bug ---");
            Console.WriteLine($"Attempting to order '{headPhones.Name}' ($ {headPhones.Price:F2}) which is below min order ($ 20.00)");
            var cartD = new Cart();
            cartD.AddItem(headPhones, 1);
            try
            {
                var order = orderService.PlaceOrder(cartD);
                Console.WriteLine($"[BUG DETECTED] SUCCESS: Order {order.OrderId} placed for $ {order.TotalAmount:F2} (below $ 20.00)!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("          End of E-Commerce Demo Application       ");
            Console.WriteLine("==================================================");
        }
    }
}
