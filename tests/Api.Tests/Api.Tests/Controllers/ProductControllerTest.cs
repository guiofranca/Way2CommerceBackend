using Api.Controllers;
using Api.DTO.Product;
using Domain.Models;
using Domain.Models.Relations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Api.Tests.Controllers;

public class ProductControllerTest
{
    private ProductController productController { get; set; }
    private ProductRequest productRequest { get; set; }
    private ProductResponse productResponse { get; set; }

    public ProductControllerTest()
    {
        productRequest = new ProductRequest("000001", "Product", "This is a product", 10.00m, new List<int> { 1, 2 });
        Product product = productRequest.MakeProductFromRequest();
        Category category1 = new Category() { Id = 1, Name = "Category1" };
        Category category2 = new Category() { Id = 2, Name = "Category2" };

        productResponse = new ProductResponse(product);

        IProductRepository repository = MockRepository();
        productController = new ProductController(repository);
    }

    private IProductRepository MockRepository()
    {
        IProductRepository repository = Substitute.For<IProductRepository>();

        repository.GetAllAsync()
            .Returns(new List<Product>
            {
                new Product(1, "0000001", "Product1", "Product1 description", 10.00m),
                new Product(1, "0000002", "Product2", "Product2 description", 10.00m),
                new Product(1, "0000003", "Product3", "Product3 description", 10.00m),
            });

        repository.CreateAsync(Arg.Any<Product>())
            .Returns(1);

        return repository;
    }

    [Fact]
    public async void CreateReturnsInt()
    {
        var response = await productController.Create(productRequest);
        var actionResult = response.Result as ObjectResult;

        Assert.IsType<ActionResult<int>>(response);
        Assert.Equal(201, actionResult.StatusCode);
        var responseContent = Assert.IsAssignableFrom<int>(actionResult.Value);
        Assert.Equal(1, responseContent);
    }

    [Fact]
    public async void GetReturnsListOfProductResponse()
    {
        var response = await productController.Get();
        var actionResult = response.Result as ObjectResult;

        Assert.IsType<ActionResult<IEnumerable<ProductResponse>>>(response);
        Assert.Equal(200, actionResult.StatusCode);
        var responseContent = Assert.IsAssignableFrom<IEnumerable<ProductResponse>>(actionResult.Value);
        Assert.Equal(3, responseContent.Count());
    }
}
