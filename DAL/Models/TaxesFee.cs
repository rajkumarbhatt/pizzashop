using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class TaxesFee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? TaxType { get; set; }

    public decimal Amount { get; set; }

    public bool? IsEnabled { get; set; }

    public bool? IsDefault { get; set; }

    public short CreatedBy { get; set; }

    public short? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<OrderTaxis> OrderTaxes { get; set; } = new List<OrderTaxis>();
}
