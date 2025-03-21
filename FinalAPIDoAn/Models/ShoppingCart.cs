using System;
using System.Collections.Generic;

namespace FinalAPIDoAn.Models;

public partial class ShoppingCart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
