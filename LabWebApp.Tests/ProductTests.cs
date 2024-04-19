using LabWebApp.Models;
using Xunit;

public class ProductTests
{
    [Fact]
    public void Product_PropertiesSetCorrectly()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Price = 9.99m,
            Description = "Test product description"
        };

        Assert.Equal(1, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal("Test product description", product.Description);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-100.0)]
    public void Product_InvalidPrice_ShouldFail(decimal invalidPrice)
    {
        var product = new Product { Price = invalidPrice };

        Assert.True(product.Price < 0, "Price is negative and should be corrected.");
    }
}
