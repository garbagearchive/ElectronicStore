using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Index("DiscountId", Name = "IX_ProductDiscounts_DiscountID")]
[Index("ProductId", Name = "IX_ProductDiscounts_ProductID")]
public partial class ProductDiscount
{
    [Key]
    [Column("ProductDiscountID")]
    public int ProductDiscountId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("DiscountID")]
    public int? DiscountId { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("ProductDiscounts")]
    public virtual Discount? Discount { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductDiscounts")]
    public virtual Product? Product { get; set; }
}
