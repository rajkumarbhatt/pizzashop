using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CustomerReview
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int OrderId { get; set; }

    public int? Food { get; set; }

    public int? Service { get; set; }

    public int? Ambience { get; set; }

    public int? AverageRating { get; set; }

    public string? Comment { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
