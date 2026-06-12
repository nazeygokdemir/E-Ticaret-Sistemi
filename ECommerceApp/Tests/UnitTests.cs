using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests
{
    [TestFixture]
    public class UnitTests
    {
        // 1. Cart_AddItem_ValidProduct_AddsItemSuccessfully (Equivalence Partitioning: Valid Input)
        [Test]
        public void Cart_AddItem_ValidProduct_AddsItemSuccessfully()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Test Product", 10.00m, 10);

            // Act
            cart.AddItem(product, 2);

            // Assert
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.Items[0].Product.Id, Is.EqualTo(1));
            Assert.That(cart.Items[0].Quantity, Is.EqualTo(2));
        }

        // 2. Cart_AddItem_NullProduct_ThrowsArgumentNullException (Branch Coverage: Null Check)
        [Test]
        public void Cart_AddItem_NullProduct_ThrowsArgumentNullException()
        {
            // Arrange
            var cart = new Cart();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cart.AddItem(null!, 1));
        }

        // 3. Cart_AddItem_ZeroOrNegativeQuantity_ThrowsArgumentException (Boundary Value Analysis: <= 0)
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public void Cart_AddItem_ZeroOrNegativeQuantity_ThrowsArgumentException(int quantity)
        {
            // Arrange
            var cart = new Cart();
            var product = new Product(1, "Test Product", 10.00m, 10);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => cart.AddItem(product, quantity));
            Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
        }

        // 4. Product_Constructor_NegativePrice_ThrowsArgumentException (Boundary Value Analysis: Price < 0)
        [Test]
        public void Product_Constructor_NegativePrice_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Product(1, "Negative Price", -0.01m, 10));
            Assert.That(ex.Message, Does.Contain("Price cannot be negative"));
        }

        // 5. Product_Constructor_NegativeStock_ThrowsArgumentException (Boundary Value Analysis: Stock < 0)
        [Test]
        public void Product_Constructor_NegativeStock_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Product(1, "Negative Stock", 10.00m, -1));
            Assert.That(ex.Message, Does.Contain("Stock cannot be negative"));
        }

        // 6. Product_ReduceStock_QuantityLessThanZero_ThrowsArgumentException (Boundary Value Analysis: Quantity <= 0)
        [TestCase(0)]
        [TestCase(-5)]
        public void Product_ReduceStock_QuantityLessThanZero_ThrowsArgumentException(int reduceQty)
        {
            // Arrange
            var product = new Product(1, "Test Product", 10.00m, 10);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => product.ReduceStock(reduceQty));
            Assert.That(ex.Message, Does.Contain("Quantity must be greater than zero"));
        }
    }
}
