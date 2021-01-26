using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                return Ok("Product was created");
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        //[HttpGet]
        public async Task<IHttpActionResult> GetProductbySKU([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (sku != null)
            {
                return Ok(product);
            }
            return NotFound();
        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProduct([FromUri] string sku, [FromBody] Product updatedProduct)
        {
            if (sku != updatedProduct?.SKU)
            {
                return BadRequest("SKU does not match in Database");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Product product = await _context.Products.FindAsync(sku);
            if (product is null)
            {
                return NotFound();
            }

            product.Name = updatedProduct.Name;
            product.Cost = updatedProduct.Cost;
            product.NumberInInventory = updatedProduct.NumberInInventory;

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok("Product was updated");
            }
            return InternalServerError();

        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);
            if (product is null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);

            if (await _context.SaveChangesAsync() != 1)
            {
                return InternalServerError();
            }
            return Ok($"Produc SKU: {product.SKU} was removed from Database");
        }
    }
}
