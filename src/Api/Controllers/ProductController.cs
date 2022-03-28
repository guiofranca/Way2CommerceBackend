using Api.Requests.Product;
using Domain.Models;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    public ProductController([FromServices] IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // GET: api/<ProductController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> Get()
    {
        IEnumerable<ProductResponse> products = (await _productRepository.GetAllAsync())
            .Select(product => new ProductResponse(product));

        return Ok(products);
    }

    // GET api/<ProductController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> Get(int id)
    {
        try
        {
            ProductResponse product = new ProductResponse(await _productRepository.GetByIdAsync(id));
            return Ok(product);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //POST api/<ProductController>
    [HttpPost]
    [Authorize(Roles = "Moderator,Administrator")]
    public async Task<ActionResult<int>> Create(ProductRequest productRequest)
    {
        //if(ModelState.IsValid) return Created(nameof(Get), productRequest);
        Product product = productRequest.MakeProductFromRequest();
        int id = await _productRepository.CreateAsync(product);
        await _productRepository.SyncCategoriesAsync(product, productRequest.CategoryIds);

        return Created("", id);
    }

    //PATCH api/<ProductController>/5
    [HttpPatch("{id}")]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<ActionResult> Patch(int id, ProductRequest productRequest)
    {
        Product product = productRequest.MakeProductFromRequest(id);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SyncCategoriesAsync(product, productRequest.CategoryIds);
        return Ok();
    }

    //DELETE api/<ProductController>/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            Product product = await _productRepository.GetByIdAsync(id);
            if(await _productRepository.DeleteAsync(product)) return Ok();
            return BadRequest("Unable to delete");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //POST api/<ProductController>/5/restore
    [HttpPost("{id}/restore")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Restore(int id)
    {
        try
        {
            bool restored = await _productRepository.RestoreAsync(id);
            if(restored) return Ok();
            return BadRequest("Unable to restore");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
