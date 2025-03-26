using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Index("DiscountCode", Name = "UQ__Discount__A1120AF5659C71A2", IsUnique = true)]
public partial class Discount
{
    [Key]
    [Column("DiscountID")]
    public int DiscountId { get; set; }

    [StringLength(50)]
    public string DiscountCode { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DiscountPercentage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
}
