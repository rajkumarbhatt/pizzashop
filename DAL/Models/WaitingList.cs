using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class WaitingList
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int? SectionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public short NoOfPersons { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Section? Section { get; set; }

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
