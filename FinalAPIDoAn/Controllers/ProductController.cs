using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public ProductController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllProduct()
        {
            var products = _dbc.Products.ToList();
            return Ok(new { data = products });
        }

        [HttpGet("Search")]
        public IActionResult SearchProduct([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Invalid search keyword." });
            }

            var results = _dbc.Products
                .Where(p => p.ProductName.Contains(keyword) || p.Description.Contains(keyword))
                .ToList();

            if (!results.Any())
            {
                return NotFound(new { message = "No products found matching the keyword." });
            }

            return Ok(new { data = results });
        }

        [HttpGet("Get/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _dbc.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            return Ok(new { data = product });
        }

        [HttpPost("Add")]
        public IActionResult AddProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryId = productDto.CategoryID,
                ImageUrl = productDto.ImageURL
            };

            _dbc.Products.Add(product);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetAllProduct), new { id = product.ProductId }, product);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _dbc.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            product.ProductName = productDto.ProductName;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.CategoryId = productDto.CategoryID;
            product.ImageUrl = productDto.ImageURL;

            _dbc.Products.Update(product);
            _dbc.SaveChanges();

            return Ok(new { data = product });
        }

        [HttpDelete("Delete/{productid}")]
        public IActionResult DeleteProduct(int productid)
        {
            var product = _dbc.Products.FirstOrDefault(p => p.ProductId == productid);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            _dbc.Products.Remove(product);
            _dbc.SaveChanges();
            return Ok(new { message = "Product deleted successfully." });
        }
    }

    public class ProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public  required string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be greater than 0.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        public  required string ImageURL { get; set; }
    }
}
