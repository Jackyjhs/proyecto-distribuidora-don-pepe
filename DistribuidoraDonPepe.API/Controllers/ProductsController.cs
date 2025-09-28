using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DistribuidoraDonPepe.API.Data;
using DistribuidoraDonPepe.API.Models;

namespace DistribuidoraDonPepe.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly DistribuidoraDonPepeContext _context;

    public ProductsController(DistribuidoraDonPepeContext context)
    {
        _context = context;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        product.UpdatedDate = DateTime.UtcNow;
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Soft delete
        product.IsActive = false;
        product.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Products/category/Granos
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
    {
        return await _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower() && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // GET: api/Products/low-stock
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<Product>>> GetLowStockProducts()
    {
        return await _context.Products
            .Where(p => p.Stock < 20 && p.IsActive)
            .OrderBy(p => p.Stock)
            .ToListAsync();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}