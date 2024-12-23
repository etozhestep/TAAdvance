using DotnetUnitTesting;

namespace UnitTests.Tests;

public class Tests
{
    private Product _product;
    private ShoppingCart _shoppingCart;

    [SetUp]
    public void Setup()
    {
        _shoppingCart = new ShoppingCart();
        _product = new Product(1, "Test Product", 5.0, 4);
    }

    [Test]
    public void Cart_AddProductToCart_CartContainsProduct()
    {
        _shoppingCart.AddProductToCart(_product);

        Assert.Multiple(() =>
            {
                Assert.That(_shoppingCart.Products, Does.Contain(_product));
                Assert.That(_shoppingCart.Products, Has.Count.EqualTo(1));
            }
        );
    }

    [Test]
    public void Cart_AddProductToCardWithSameId_IncreasesQuantity()
    {
        _shoppingCart.AddProductToCart(_product);
        _shoppingCart.AddProductToCart(new Product(1, "Test Product", 5.0, 4));

        Assert.That(_shoppingCart.GetProductById(1).Quantity, Is.EqualTo(8));
    }

    [Test]
    public void Cart_RemoveProductFromCart_ProductIsAbsentInCart()
    {
        _shoppingCart.AddProductToCart(_product);
        _shoppingCart.RemoveProductFromCart(_product);

        Assert.Multiple(() =>
            {
                Assert.That(_shoppingCart.Products, Does.Not.Contain(_product));
                Assert.That(_shoppingCart.Products, Has.Count.EqualTo(0));
            }
        );
    }

    [Test]
    public void Cart_RemoveAllProductsById_AllProductsRemoved()
    {
        _shoppingCart.AddProductToCart(_product);
        _shoppingCart.AddProductToCart(new Product(1, "Test Product", 5.0, 4));
        _shoppingCart.RemoveProductFromCart(_product);

        Assert.Throws<ProductNotFoundException>(() => _shoppingCart.GetProductById(1));
    }

    [Test]
    public void Cart_RemoveNotExistingProduct_ThrewException()
    {
        var notExistingProduct = new Product(1, "Not exist", 7.0, 100);

        Assert.Throws<ProductNotFoundException>(() => _shoppingCart.RemoveProductFromCart(notExistingProduct));
    }

    [Test]
    public void Cart_GetTotalPriceOfProducts_ReturnedTotalPrice()
    {
        _shoppingCart.AddProductToCart(new Product(1, "First Product", 10.0, 5));
        _shoppingCart.AddProductToCart(new Product(2, "Second Product", 20.0, 2));

        var actualTotalPrice = _shoppingCart.GetCartTotalPrice();
        const int expectedTotalPrice = 90;

        Assert.That(actualTotalPrice, Is.EqualTo(expectedTotalPrice));
    }
}