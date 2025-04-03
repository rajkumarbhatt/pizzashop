using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CustomerFavourite
{
    public int ItemId { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public int Id { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
