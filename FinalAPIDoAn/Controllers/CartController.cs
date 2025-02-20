using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var shoppingCarts = _dbc.ShoppingCarts.ToList();
            return Ok(new { data = shoppingCarts });
        }
        [HttpGet("Search")]
        public IActionResult GetCartDetailById(int id)
        {
            var shoppingCarts = _dbc.ShoppingCarts.SingleOrDefault(o => o.CartId == id);
            if (shoppingCarts == null) return NotFound(new { message = "Cart not found." });

            return Ok(new { data = shoppingCarts });
        }
        [HttpPost("Add")]
        public IActionResult AddCart([FromBody] CartDto cartDto)
        {
            if (cartDto.UserID <=0 || cartDto.ProductID <= 0 ||cartDto.Quantity <= 0)
            {
                return BadRequest(new { message = "Invalid cart data." });
            }
            var cart = new ShoppingCart
            {
                UserId = cartDto.UserID,
                ProductId = cartDto.ProductID,
                Quantity = cartDto.Quantity
            };
            _dbc.ShoppingCarts.Add(cart);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetCartDetailById), new { id = cartDto.CartID}, cart);
        }
        [HttpPut("Update")]
        public IActionResult Update([FromBody] CartDto cartDto)
        {
            if (cartDto.UserID <= 0 || cartDto.ProductID <= 0 || cartDto.Quantity <= 0)
            {
                return BadRequest(new { message = "Invalid cart data." });
            }
            var cart = new ShoppingCart
            {
                UserId = cartDto.UserID,
                ProductId = cartDto.ProductID,
                Quantity = cartDto.Quantity
            };
            _dbc.ShoppingCarts.Update(cart);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetCartDetailById), new { id = cartDto.CartID }, cart);
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteCart(int cartid)
        {
            var cart = _dbc.ShoppingCarts.SingleOrDefault(o => o.CartId == cartid);
            if (cart == null) return NotFound(new { message = "Cart not found." });
            _dbc.ShoppingCarts.Remove(cart);
            _dbc.SaveChanges();
            return Ok(new { message = "Cart deleted successfully." });
        }
    }
    public class CartDto {
        public required int CartID { get; set; }
        public required int UserID { get; set; }
        public required int ProductID { get; set; }
        public required int Quantity { get; set; }
        public required DateTime AddedAt { get; set; }
    }
}

