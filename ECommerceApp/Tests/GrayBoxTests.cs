using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests
{
    [TestFixture]
    public class GrayBoxTests
    {
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _orderService = new OrderService();
        }

        // 13. OrderService_PlaceOrder_UpdatesStockQuantityCorrectly (State verification: Product stock reduction)
        [Test]
        public void OrderService_PlaceOrder_UpdatesStockQuantityCorrectly()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Product Gray A", 50.00m, 10);
            cart.AddItem(product, 2);

            // Act
            var order = _orderService.PlaceOrder(cart);

            // Assert
            Assert.That(order, Is.Not.Null);
            Assert.That(product.Stock, Is.EqualTo(8), "Product stock should be reduced by the ordered quantity (10 - 2 = 8).");
        }

        // 14. OrderService_PlaceOrder_InvalidDiscountCode_ThrowsException (Validation path testing)
        [Test]
        public void OrderService_PlaceOrder_InvalidDiscountCode_ThrowsException()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Product Gray B", 50.00m, 5);
            cart.AddItem(product, 1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _orderService.PlaceOrder(cart, "INVALID_CODE"));
            Assert.That(ex.Message, Does.Contain("Invalid discount code"));
        }

        // 15. OrderService_PlaceOrder_EmptyCart_ThrowsInvalidOperationException (State validation testing)
        [Test]
        public void OrderService_PlaceOrder_EmptyCart_ThrowsInvalidOperationException()
        {
            // Arrange
            var cart = new Cart(); // Empty cart

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(cart));
            Assert.That(ex.Message, Does.Contain("Cannot place order with an empty cart"));
        }

        // 16. OrderService_PlaceOrder_InsufficientStockButAboveZero_ThrowsException (Boundary State verification: Stock > 0 but < Quantity)
        // NOTE: This test is EXPECTED TO FAIL because of the Stock Bug (only checks Stock < 0, allowing stock to go negative).
        [Test]
        public void OrderService_PlaceOrder_InsufficientStockButAboveZero_ThrowsException()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Product Gray C", 50.00m, 2); // Stock is 2
            cart.AddItem(product, 3); // Quantity ordered is 3 (3 > 2, so it should fail)

            // Act & Assert
            // Expecting stock check to throw an exception because demand exceeds supply.
            // Due to BUG #2, the stock is not checked against the requested quantity, allowing stock to drop to -1.
            // Thus, this test will FAIL because no exception is thrown.
            Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(cart),
                "Order should fail because quantity requested (3) is greater than available stock (2).");
        }

        // 17. OrderService_PlaceOrder_MultipleDifferentItems_DeductsStockForAll (Multi-component state verification)
        [Test]
        public void OrderService_PlaceOrder_MultipleDifferentItems_DeductsStockForAll()
        {
            // Arrange
            var cart = new Cart();
            var prod1 = new Product(1, "Prod 1", 30.00m, 5);
            var prod2 = new Product(2, "Prod 2", 40.00m, 10);
            
            cart.AddItem(prod1, 2);
            cart.AddItem(prod2, 3);

            // Act
            var order = _orderService.PlaceOrder(cart);

            // Assert
            Assert.That(order, Is.Not.Null);
            Assert.That(prod1.Stock, Is.EqualTo(3), "Prod 1 stock should reduce from 5 to 3.");
            Assert.That(prod2.Stock, Is.EqualTo(7), "Prod 2 stock should reduce from 10 to 7.");
        }
    }
}
