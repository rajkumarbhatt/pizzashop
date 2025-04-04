using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? InvoiceNo { get; set; }

    public virtual Order Order { get; set; } = null!;
}
