using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class WaitingList
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int? TableId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Table? Table { get; set; }

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
