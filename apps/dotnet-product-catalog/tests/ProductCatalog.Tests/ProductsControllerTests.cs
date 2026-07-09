using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalog.Api.Controllers;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Services;
using Xunit;

namespace ProductCatalog.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public void GetAll_ReturnsOkWithProducts()
    {
        _serviceMock.Setup(s => s.GetAll()).Returns(new List<Product> { new() { Id = 1, Name = "Monitor" } });

        var result = _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Single(products);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetById(5)).Returns((Product?)null);

        var result = _controller.GetById(5);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void Create_ReturnsBadRequest_WhenServiceThrows()
    {
        var product = new Product { Name = "" };
        _serviceMock.Setup(s => s.Create(product)).Throws(new ArgumentException("Product name is required."));

        var result = _controller.Create(product);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
