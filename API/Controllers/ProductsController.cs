using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository _repository) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        return Ok(await _repository.GetProductAsync(brand, type, sort));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repository.GetProductByIdAsyc(id);

        if(product == null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _repository.AddProduct(product);

        if(await _repository.SaveChangesAsync()) 
        {
            return CreatedAtAction("GetProduct", new {id = product.Id},product);
        }

        return BadRequest("Problem on creating product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product) 
    {
        if(product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product!");

        _repository.UpdateProduct(product);

       if(await _repository.SaveChangesAsync()) 
       {
        return NoContent();
       }

        return BadRequest("Problem on updating the product");
    }

    [HttpDelete("{id:int}")]

    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _repository.GetProductByIdAsyc(id);

        if(product == null) return NotFound();

        _repository.DeleteProduct(product);

        if(await _repository.SaveChangesAsync()) 
       {
        return NoContent();
       }

        return BadRequest("Problem on deleting the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands() 
    {
        return Ok(await _repository.GetBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes() 
    {
        return Ok(await _repository.GetTypeAsync());
    }

    private bool ProductExist(int id) {
        return _repository.ProductExist(id);
    }
}
