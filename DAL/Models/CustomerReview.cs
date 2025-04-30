using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CustomerReview
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int OrderId { get; set; }

    public short Food { get; set; }

    public short Service { get; set; }

    public short Ambience { get; set; }

    public decimal AverageRating { get; set; }

    public string? Comment { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
