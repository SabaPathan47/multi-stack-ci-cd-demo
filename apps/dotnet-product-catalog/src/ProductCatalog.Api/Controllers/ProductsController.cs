using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _service.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        try
        {
            var created = _service.Create(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        return _service.Update(id, product) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return _service.Delete(id) ? NoContent() : NotFound();
    }
}
