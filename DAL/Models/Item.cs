using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? Description { get; set; }

    public string ItemType { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public bool? IsAvailable { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? DefaultTax { get; set; }

    public decimal TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ItemModifiergroup> ItemModifiergroups { get; set; } = new List<ItemModifiergroup>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
