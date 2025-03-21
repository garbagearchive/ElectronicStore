using System;
using System.Collections.Generic;

namespace FinalAPIDoAn.Models;

public partial class ProductRepair
{
    public int RepairId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public DateTime? RepairRequestDate { get; set; }

    public string? IssueDescription { get; set; }

    public string? RepairStatus { get; set; }

    public DateTime? RepairCompletionDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
