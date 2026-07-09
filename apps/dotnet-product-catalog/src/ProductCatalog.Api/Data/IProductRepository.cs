using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Data;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    Product Add(Product product);
    bool Update(int id, Product product);
    bool Delete(int id);
}
