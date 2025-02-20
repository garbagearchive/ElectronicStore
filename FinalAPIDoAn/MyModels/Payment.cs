using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.MyModels;

public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(20)]
    public string? PaymentStatus { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order? Order { get; set; }
}
