using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ModifierModifiergroupMapping
{
    public int ModifierId { get; set; }

    public int ModifiergroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual ModifierGroup Modifiergroup { get; set; } = null!;
}
