using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.MyModels;

[Table("ShoppingCart")]
[Index("UserId", Name = "IX_ShoppingCart_UserID")]
public partial class ShoppingCart
{
    [Key]
    [Column("CartID")]
    public int CartId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AddedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ShoppingCarts")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ShoppingCarts")]
    public virtual User? User { get; set; }
}
