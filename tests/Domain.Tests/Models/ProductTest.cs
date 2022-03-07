using Domain.Models;
using Xunit;
using System;

namespace Domain.Tests.Models;

public class ProductTest
{
    private Product _product { get; set; }

    public ProductTest()
    {
        _product = new Product("000001", "Product", "Test product", (decimal)15.25);
    }

    [Fact]
    public void HavePropertyName()
    {
        Assert.Equal("Product", _product.Name);
    }
}
