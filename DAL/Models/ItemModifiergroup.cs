using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ItemModifiergroup
{
    public int ItemId { get; set; }

    public int ModifiergroupId { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public short? MinValue { get; set; }

    public short? MaxValue { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual ModifierGroup Modifiergroup { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
