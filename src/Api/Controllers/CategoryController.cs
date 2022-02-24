using Api.DTO.Category;
using Domain.Models;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryController([FromServices] ICategoryRepository repo)
    {
        _categoryRepository = repo;
    }

    // GET: api/<CategoryController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get()
    {
        IEnumerable<CategoryResponse> categories = (await _categoryRepository.GetAllAsync())
            .Select(category => new CategoryResponse(category));

        return Ok(categories);
    }

    // GET api/<CategoryController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> Get(int id)
    {
        try
        {
            CategoryResponse category = new CategoryResponse(await _categoryRepository.GetByIdAsync(id));
            return Ok(category);
        }
        catch 
        {
            return NotFound();
        }
    }
}
