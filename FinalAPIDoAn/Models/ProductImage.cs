using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Index("ProductId", Name = "IX_ProductImages_ProductID")]
public partial class ProductImage
{
    [Key]
    [Column("ImageID")]
    public int ImageId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("ImageURL")]
    [StringLength(255)]
    public string ImageUrl { get; set; } = null!;

    [Column("ThumbnailURL")]
    [StringLength(255)]
    public string? ThumbnailUrl { get; set; }

    public int? ImageOrder { get; set; }

    [Column("PublicId")]  // Explicit column mapping
    [StringLength(255)]   // Match the database column size
    public string PublicId { get; set; } = null!;  // Changed to public set


    public bool? IsDefault { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}
