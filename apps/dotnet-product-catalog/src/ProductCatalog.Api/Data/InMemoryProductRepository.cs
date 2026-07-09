using System.Collections.Concurrent;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Data;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _store = new();
    private int _nextId = 1;

    public IEnumerable<Product> GetAll() => _store.Values;

    public Product? GetById(int id) => _store.GetValueOrDefault(id);

    public Product Add(Product product)
    {
        product.Id = Interlocked.Increment(ref _nextId) - 1;
        _store[product.Id] = product;
        return product;
    }

    public bool Update(int id, Product product)
    {
        if (!_store.ContainsKey(id)) return false;
        product.Id = id;
        _store[id] = product;
        return true;
    }

    public bool Delete(int id) => _store.TryRemove(id, out _);
}
