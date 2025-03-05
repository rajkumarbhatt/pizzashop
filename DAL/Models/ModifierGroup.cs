using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ModifierGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ItemModifiergroup> ItemModifiergroups { get; set; } = new List<ItemModifiergroup>();

    public virtual ICollection<ModifierModifiergroupMapping> ModifierModifiergroupMappings { get; set; } = new List<ModifierModifiergroupMapping>();

    public virtual User UpdatedByNavigation { get; set; } = null!;

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
