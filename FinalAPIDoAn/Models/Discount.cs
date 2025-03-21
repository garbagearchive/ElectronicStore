using System;
using System.Collections.Generic;

namespace FinalAPIDoAn.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountCode { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
}
