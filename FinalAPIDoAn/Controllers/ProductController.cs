using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
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
                var order = _dbc.Orders.ToList();
                return Ok(new { data = order });
            }

            [HttpGet("Search")]
            public IActionResult SearchProduct([FromQuery] string keyword)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "Invalid search keyword" });
                }
                var results = _dbc.Products.Where(s => s.ProductName.Contains(keyword));

                if (!results.Any())
                {
                    return NotFound(new { message = "No product found matching the keyword." });
                }

                return Ok(new { data = results });
            }

            [HttpPost("Add")]
            public IActionResult AddProduct([FromBody] ProductDto productDto)
            {
                if (string.IsNullOrWhiteSpace(productDto.ProductName) || productDto.Price <= 0 || productDto.StockQuantity <= 0 || productDto.CategoryID <= 0)
                {
                    return BadRequest(new { message = "Invalid product data." });
                }
                var product = new Product
                {
                    ProductId = productDto.ProductID,
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    StockQuantity = productDto.StockQuantity,
                    CategoryId = productDto.CategoryID,
                    ImageUrl = productDto.ImageURL
                };
                _dbc.Products.Add(product);
                _dbc.SaveChanges();
                return CreatedAtAction(nameof(SearchProduct), new { id = product.ProductId }, product);
            }
            [HttpPut("Update")]
            public IActionResult UpdateProduct([FromBody] ProductDto productDto)
            {
                if (string.IsNullOrWhiteSpace(productDto.ProductName) || productDto.Price <= 0 || productDto.StockQuantity <= 0 || productDto.CategoryID <= 0)
                {
                    return BadRequest(new { message = "Invalid product data." });
                }
                var product = new Product
                {
                    ProductId = productDto.ProductID,
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    StockQuantity = productDto.StockQuantity,
                    CategoryId = productDto.CategoryID,
                    ImageUrl = productDto.ImageURL
                };
                _dbc.Products.Update(product);
                _dbc.SaveChanges();
                return CreatedAtAction(nameof(SearchProduct), new { id = product.ProductId }, product);
            }
            [HttpDelete("Delete")]
            public IActionResult DeleteProduct(int productid)
            {
                var product = _dbc.Products.SingleOrDefault(o => o.ProductId == productid);
                if (product == null) return NotFound(new { message = "Order not found." });
                _dbc.Products.Remove(product);
                _dbc.SaveChanges();
                return Ok(new { message = "Product deleted successfully." });
            }
        }
    public class ProductDto
    {
        public required int ProductID { get; set; }
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required int Price { get; set; }
        public required int StockQuantity { get; set; }
        public required int CategoryID { get; set; }
        public required string ImageURL { get; set; }
    }

}


