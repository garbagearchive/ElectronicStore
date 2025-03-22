using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public CartController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllCartDetails()
        {
            var shoppingCarts = _dbc.ShoppingCarts
                .Join(
                    _dbc.Users,
                    cart => cart.UserId,
                    user => user.UserId,
                    (cart, user) => new { Cart = cart, User = user }
                ).Join(
                    _dbc.Products,
                    combined => combined.Cart.ProductId,
                    product => product.ProductId,
                    (combined, product) => new
                    {
                        CartId = combined.Cart.CartId,
                        UserId = combined.Cart.UserId,
                        Username = combined.User.Username,
                        ProductId = combined.Cart.ProductId,
                        ProductName = product.ProductName,
                        Quantity = combined.Cart.Quantity,
                        AddedAt = combined.Cart.AddedAt
                    }
                ).ToList();

            return Ok(new { data = shoppingCarts });
        }

        [HttpGet("Search")]
        public IActionResult GetCartDetailById(int id)
        {
            var shoppingCart = _dbc.ShoppingCarts.SingleOrDefault(o => o.CartId == id);
            if (shoppingCart == null) return NotFound(new { message = "Cart not found." });

            return Ok(new { data = shoppingCart });
        }

        [HttpPost("Add")]
        public IActionResult AddCart([FromBody] CartDto cartDto)
        {
            if (cartDto == null)
                return BadRequest(new { message = "Invalid cart data." });

            // Check if the UserID exists
            var userExists = _dbc.Users.Any(u => u.UserId == cartDto.UserID);
            if (!userExists)
                return BadRequest(new { message = "Invalid UserID. The user does not exist." });

            // Check if the ProductID exists
            var productExists = _dbc.Products.Any(p => p.ProductId == cartDto.ProductID);
            if (!productExists)
                return BadRequest(new { message = "Invalid ProductID. The product does not exist." });

            // Validate Quantity
            if (cartDto.Quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than 0." });

            var cart = new ShoppingCart
            {
                UserId = cartDto.UserID,
                ProductId = cartDto.ProductID,
                Quantity = cartDto.Quantity,
                AddedAt = DateTime.UtcNow
            };

            _dbc.ShoppingCarts.Add(cart);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetCartDetailById), new { id = cart.CartId }, cart);
        }

        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, [FromBody] CartDto cartDto)
        {
            if (cartDto == null)
                return BadRequest(new { message = "Invalid cart data." });

            var cart = _dbc.ShoppingCarts.SingleOrDefault(o => o.CartId == id);
            if (cart == null) return NotFound(new { message = "Cart not found." });

            if (cartDto.UserID <= 0 || cartDto.ProductID <= 0 || cartDto.Quantity <= 0)
                return BadRequest(new { message = "Invalid cart data." });

            cart.UserId = cartDto.UserID;
            cart.ProductId = cartDto.ProductID;
            cart.Quantity = cartDto.Quantity;

            _dbc.ShoppingCarts.Update(cart);
            _dbc.SaveChanges();

            return Ok(new { message = "Cart updated successfully.", data = cart });
        }

        [HttpDelete("Delete/{cartid}")]
        public IActionResult DeleteCart(int cartid)
        {
            var cart = _dbc.ShoppingCarts.SingleOrDefault(o => o.CartId == cartid);
            if (cart == null) return NotFound(new { message = "Cart not found." });

            _dbc.ShoppingCarts.Remove(cart);
            _dbc.SaveChanges();

            return Ok(new { message = "Cart deleted successfully." });
        }
    }

    public class CartDto
    {
        public int UserID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductID must be greater than 0.")]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
