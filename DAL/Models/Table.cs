using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Table
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SectionId { get; set; }

    public int Capacity { get; set; }

    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Section Section { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;

    public virtual ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
}
