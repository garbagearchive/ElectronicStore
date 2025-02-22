using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.MyModels;

[Index("ProductId", Name = "IX_ProductReviews_ProductID")]
[Index("UserId", Name = "IX_ProductReviews_UserID")]
public partial class ProductReview
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    public int? Rating { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ProductReviews")]
    public virtual User? User { get; set; }
}
