using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests
{
    [TestFixture]
    public class BlackBoxTests
    {
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _orderService = new OrderService();
        }

        // 7. OrderService_PlaceOrder_AmountExactlyAtMinimum_Succeeds (Boundary Value Analysis: Minimum total = $20.00)
        [Test]
        public void OrderService_PlaceOrder_AmountExactlyAtMinimum_Succeeds()
        {
            // Arrange
            var cart = new Cart();
            // Price is 20.00, quantity is 1 -> total = 20.00 (exactly at the minimum limit)
            var product = new Product(1, "Product A", 20.00m, 5);
            cart.AddItem(product, 1);

            // Act & Assert
            Assert.DoesNotThrow(() => _orderService.PlaceOrder(cart));
        }

        // 8. OrderService_PlaceOrder_AmountBelowMinimum_ThrowsException (Equivalence Partitioning & BVA: Total < $20.00)
        // NOTE: This test is EXPECTED TO FAIL because of the Minimum Order Bug (only prints warning instead of throwing).
        [Test]
        public void OrderService_PlaceOrder_AmountBelowMinimum_ThrowsException()
        {
            // Arrange
            var cart = new Cart();
            // Price is 19.99 (boundary), total = 19.99 (below $20.00)
            var product = new Product(1, "Product B", 19.99m, 5);
            cart.AddItem(product, 1);

            // Act & Assert
            // This assertion expects an exception since $19.99 is below the $20.00 minimum limit.
            // Due to BUG #3, the exception is NOT thrown, causing this test to FAIL.
            Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(cart), 
                "Order should fail because the total is below the minimum limit of $20.00.");
        }

        // 9. OrderService_PlaceOrder_AmountAboveMinimum_Succeeds (Equivalence Partitioning: Total > $20.00)
        [Test]
        public void OrderService_PlaceOrder_AmountAboveMinimum_Succeeds()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Product C", 25.00m, 5);
            cart.AddItem(product, 1);

            // Act
            var order = _orderService.PlaceOrder(cart);

            // Assert
            Assert.That(order, Is.Not.Null);
            Assert.That(order.TotalAmount, Is.EqualTo(25.00m));
        }

        // 10. OrderService_PlaceOrder_ProductOutOfStock_ThrowsException (Equivalence Partitioning & BVA: Stock = 0)
        // NOTE: This test is EXPECTED TO FAIL because of the Stock Bug (checks Stock < 0, accepting 0 stock).
        [Test]
        public void OrderService_PlaceOrder_ProductOutOfStock_ThrowsException()
        {
            // Arrange
            var cart = new Cart();
            // Out of stock product (Stock = 0, boundary condition)
            var product = new Product(1, "Out of Stock Product", 30.00m, 0);
            cart.AddItem(product, 1);

            // Act & Assert
            // Placing order for an item with 0 stock should throw an exception.
            // Due to BUG #2, the system accepts 0 stock and completes the order, causing this test to FAIL.
            Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(cart),
                "Order should fail because the product stock is 0.");
        }

        // 11. OrderService_PlaceOrder_ProductInStock_Succeeds (Equivalence Partitioning: Stock > 0)
        [Test]
        public void OrderService_PlaceOrder_ProductInStock_Succeeds()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "In Stock Product", 30.00m, 1); // Stock exactly 1 (boundary)
            cart.AddItem(product, 1);

            // Act & Assert
            Assert.DoesNotThrow(() => _orderService.PlaceOrder(cart));
        }

        // 12. OrderService_PlaceOrder_ValidDiscountCode_AppliesCorrectDiscount (Equivalence Partitioning: Valid Discount)
        // NOTE: This test is EXPECTED TO FAIL because of the Discount Bug (applies 50% discount instead of 10% for SAVE10).
        [Test]
        public void OrderService_PlaceOrder_ValidDiscountCode_AppliesCorrectDiscount()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Expensive Product", 100.00m, 5);
            cart.AddItem(product, 1);

            // Act
            var order = _orderService.PlaceOrder(cart, "SAVE10");

            // Assert
            // Expected discount: 10% of $100.00 = $10.00. Expected total: $90.00.
            // Due to BUG #1, it applies 50% ($50.00 discount, total $50.00), making this assertion FAIL.
            Assert.That(order.TotalAmount, Is.EqualTo(90.00m), "SAVE10 should apply a 10% discount.");
            Assert.That(order.DiscountApplied, Is.EqualTo(10.00m));
        }
    }
}
