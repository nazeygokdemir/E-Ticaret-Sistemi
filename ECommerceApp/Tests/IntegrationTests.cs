using System;
using System.Linq;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _orderService = new OrderService();
        }

        // 18. OrderService_Integration_SuccessfulOrderFlow_StockReducedAndOrderSaved (Full flow verification)
        [Test]
        public void OrderService_Integration_SuccessfulOrderFlow_StockReducedAndOrderSaved()
        {
            // Arrange
            var cart = new Cart();
            var prod1 = new Product(1, "Integration Product A", 25.00m, 10);
            var prod2 = new Product(2, "Integration Product B", 35.00m, 5);

            cart.AddItem(prod1, 2);
            cart.AddItem(prod2, 1); // Total = (25*2) + (35*1) = $85.00

            // Act
            var order = _orderService.PlaceOrder(cart);

            // Assert
            Assert.That(order, Is.Not.Null);
            Assert.That(_orderService.Orders, Has.Count.EqualTo(1));
            Assert.That(_orderService.Orders[0].OrderId, Is.EqualTo(order.OrderId));
            Assert.That(_orderService.Orders[0].TotalAmount, Is.EqualTo(85.00m));
            
            // Verify Stock updates
            Assert.That(prod1.Stock, Is.EqualTo(8));
            Assert.That(prod2.Stock, Is.EqualTo(4));
        }

        // 19. OrderService_Integration_PaymentFails_RollbacksStockCorrectly (Transactional consistency testing)
        [Test]
        public void OrderService_Integration_PaymentFails_RollbacksStockCorrectly()
        {
            // Arrange
            var cart = new Cart();
            // Price is $999.00 to trigger payment failure simulator in OrderService.
            var expensiveProd = new Product(1, "Extremely Expensive TV", 999.00m, 3);
            cart.AddItem(expensiveProd, 1);

            int initialStock = expensiveProd.Stock;

            // Act & Assert
            // 1. Placing the order should throw InvalidOperationException because payment simulation fails for $999.00
            var ex = Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(cart));
            Assert.That(ex.Message, Does.Contain("Payment failed"));

            // 2. Order should NOT be saved in the system
            Assert.That(_orderService.Orders, Has.Count.EqualTo(0));

            // 3. Stock should be rolled back to initial value (3) instead of remaining reduced (2)
            Assert.That(expensiveProd.Stock, Is.EqualTo(initialStock), "Product stock should be rolled back after payment failure.");
        }

        // 20. OrderService_Integration_MultipleOrders_MaintainsCorrectInventoryState (State transition testing)
        [Test]
        public void OrderService_Integration_MultipleOrders_MaintainsCorrectInventoryState()
        {
            // Arrange
            var prod = new Product(1, "Reusable Item", 25.00m, 10);

            // Order 1
            var cart1 = new Cart();
            cart1.AddItem(prod, 2);
            var order1 = _orderService.PlaceOrder(cart1);

            // Order 2
            var cart2 = new Cart();
            cart2.AddItem(prod, 3);
            var order2 = _orderService.PlaceOrder(cart2);

            // Assert
            Assert.That(_orderService.Orders, Has.Count.EqualTo(2));
            Assert.That(prod.Stock, Is.EqualTo(5), "Initial stock 10, reduced by 2 and 3 should leave 5.");
            Assert.That(_orderService.Orders[0].TotalAmount, Is.EqualTo(50.00m));
            Assert.That(_orderService.Orders[1].TotalAmount, Is.EqualTo(75.00m));
        }

        // 21. OrderService_Integration_PlaceOrder_LeavesCartIntactForReference (Cart reference verification)
        [Test]
        public void OrderService_Integration_PlaceOrder_LeavesCartIntactForReference()
        {
            // Arrange
            var cart = new Cart();
            var prod = new Product(1, "Item", 30.00m, 5);
            cart.AddItem(prod, 1);

            // Act
            var order = _orderService.PlaceOrder(cart);

            // Assert
            // Placing order does not clear the cart reference automatically so the customer could review it.
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.Items[0].Product.Id, Is.EqualTo(1));
        }
    }
}
