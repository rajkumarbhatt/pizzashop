using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Modifier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ModifierModifiergroupMapping> ModifierModifiergroupMappings { get; set; } = new List<ModifierModifiergroupMapping>();

    public virtual ICollection<OrderModifier> OrderModifiers { get; set; } = new List<OrderModifier>();

    public virtual User UpdatedByNavigation { get; set; } = null!;

    public virtual ICollection<ModifierGroup> Modifiergroups { get; set; } = new List<ModifierGroup>();
}
