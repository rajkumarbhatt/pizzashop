using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderModifier
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public int ModifierId { get; set; }

    public decimal Price { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
