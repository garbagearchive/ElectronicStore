using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Index("ProductId", Name = "IX_ProductRepairs_ProductID")]
[Index("UserId", Name = "IX_ProductRepairs_UserID")]
public partial class ProductRepair
{
    [Key]
    [Column("RepairID")]
    public int RepairId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RepairRequestDate { get; set; }

    [StringLength(500)]
    public string? IssueDescription { get; set; }

    [StringLength(50)]
    public string? RepairStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RepairCompletionDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductRepairs")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ProductRepairs")]
    public virtual User? User { get; set; }
}
