using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderTableMapping
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int TableId { get; set; }

    public int NoOfPersons { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
