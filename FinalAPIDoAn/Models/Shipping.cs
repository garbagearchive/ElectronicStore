using System;
using System.Collections.Generic;

namespace FinalAPIDoAn.Models;

public partial class Shipping
{
    public int ShippingId { get; set; }

    public int? OrderId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public string ShippingMethod { get; set; } = null!;

    public string? TrackingNumber { get; set; }

    public DateOnly? EstimatedDeliveryDate { get; set; }

    public string? ShippingStatus { get; set; }

    public virtual Order? Order { get; set; }
}
