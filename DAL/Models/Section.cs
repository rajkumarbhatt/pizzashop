using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Section
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
