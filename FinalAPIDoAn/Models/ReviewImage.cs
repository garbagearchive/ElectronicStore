using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Index("ReviewId", Name = "IX_ReviewImages_ReviewID")]
public partial class ReviewImage
{
    [Key]
    [Column("ImageID")]
    public int ImageId { get; set; }

    [Column("ReviewID")]
    public int ReviewId { get; set; }

    [Column("ImageURL")]
    [StringLength(255)]
    public string ImageUrl { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ReviewId")]
    [InverseProperty("ReviewImages")]
    public virtual ProductReview Review { get; set; } = null!;
}
