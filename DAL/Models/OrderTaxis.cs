using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderTaxis
{
    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public decimal TaxAmount { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual TaxesFee Tax { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
