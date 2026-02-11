using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagementSystem.Models;

namespace ManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.Include(p => p.Projects).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(string name, List<Guid> projectIds)
    {
        var projects = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Projects = projects
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(product);
    }
}