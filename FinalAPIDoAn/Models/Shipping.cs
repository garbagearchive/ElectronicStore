using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

[Table("Shipping")]
public partial class Shipping
{
    [Key]
    [Column("ShippingID")]
    public int ShippingId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [StringLength(255)]
    public string ShippingAddress { get; set; } = null!;

    [StringLength(50)]
    public string ShippingMethod { get; set; } = null!;

    [StringLength(50)]
    public string? TrackingNumber { get; set; }

    public DateOnly? EstimatedDeliveryDate { get; set; }

    [StringLength(20)]
    public string? ShippingStatus { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Shippings")]
    public virtual Order? Order { get; set; }
}
