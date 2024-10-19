using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Laptop", ProductCode = "LP-001", ProductPrice = 1500 },
            new Product { ProductId = 2, ProductName = "Mouse", ProductCode = "MS-002", ProductPrice = 25 }
        };

        private static int nextProductId = 3;

        // Get all products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return products;
        }

        // Get product by ID
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // Add a new product
        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            product.ProductId = nextProductId++;
            products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        // Update an existing product
        [HttpPut]
        public IActionResult Update([FromBody] Product updatedProduct)
        {
            var product = products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.ProductName = updatedProduct.ProductName;
            product.ProductCode = updatedProduct.ProductCode;
            product.ProductPrice = updatedProduct.ProductPrice;

            return Ok("Product updated successfully");
        }

        // Delete a product
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            products.Remove(product);
            return Ok("Product deleted successfully");
        }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
