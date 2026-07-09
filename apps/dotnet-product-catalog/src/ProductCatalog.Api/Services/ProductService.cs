using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Product> GetAll() => _repository.GetAll();

    public Product? GetById(int id) => _repository.GetById(id);

    public Product Create(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required.");
        if (product.Price < 0)
            throw new ArgumentException("Price cannot be negative.");
        return _repository.Add(product);
    }

    public bool Update(int id, Product product) => _repository.Update(id, product);

    public bool Delete(int id) => _repository.Delete(id);
}
