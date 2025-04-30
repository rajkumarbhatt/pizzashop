using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderTaxis
{
    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public decimal TaxAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
}
