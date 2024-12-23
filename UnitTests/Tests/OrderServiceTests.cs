using DotnetUnitTesting;
using Moq;

namespace UnitTests.Tests;

public class OrderServiceTests
{
    private Mock<IDiscountUtility> _mockDiscountUtility;
    private OrderService _orderService;
    private Product _product;
    private UserAccount _userAccount;

    [SetUp]
    public void Setup()
    {
        _mockDiscountUtility = new Mock<IDiscountUtility>();
        _orderService = new OrderService(_mockDiscountUtility.Object);
        _userAccount = new UserAccount("John", "Smith", "1990/10/10");
        _product = new Product(1, "Test Product", 25.0, 4);

        _mockDiscountUtility.Setup(discount => discount.CalculateDiscount(It.Is<UserAccount>(
            user => user.Name == _userAccount.Name 
                    && user.Surname == _userAccount.Surname 
                    && user.DateOfBirth == _userAccount.DateOfBirth 
                    && user.ShoppingCart.Products.Count > 0))).Returns(3);
    }

    [Test]
    public void Order_CalcFinalPriceWithDiscount_ReturnedDiscountedPrice()
    {
        _userAccount.ShoppingCart.AddProductToCart(_product);
        var finalPrice = _orderService.GetOrderPrice(_userAccount);

        Assert.That(finalPrice, Is.EqualTo(97));
    }
    
    [Test]
    public void Order_CalcFinalPriceWithEmptyCart_ReturnedZero()
    {
        var finalPrice = _orderService.GetOrderPrice(_userAccount);

        Assert.That(finalPrice, Is.EqualTo(0));
    }
}