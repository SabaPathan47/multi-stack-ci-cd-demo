using Moq;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Services;
using Xunit;

namespace ProductCatalog.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _service = new ProductService(_repoMock.Object);
    }

    [Fact]
    public void Create_ReturnsProduct_WhenValid()
    {
        var product = new Product { Name = "Keyboard", Price = 49.99m };
        _repoMock.Setup(r => r.Add(product)).Returns(product);

        var result = _service.Create(product);

        Assert.Equal("Keyboard", result.Name);
        _repoMock.Verify(r => r.Add(product), Times.Once);
    }

    [Fact]
    public void Create_Throws_WhenNameMissing()
    {
        var product = new Product { Name = "", Price = 10m };

        Assert.Throws<ArgumentException>(() => _service.Create(product));
    }

    [Fact]
    public void Create_Throws_WhenPriceNegative()
    {
        var product = new Product { Name = "Mouse", Price = -5m };

        Assert.Throws<ArgumentException>(() => _service.Create(product));
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        var result = _service.GetById(99);

        Assert.Null(result);
    }

    [Fact]
    public void Delete_ReturnsTrue_WhenRemoved()
    {
        _repoMock.Setup(r => r.Delete(1)).Returns(true);

        var result = _service.Delete(1);

        Assert.True(result);
    }
}
