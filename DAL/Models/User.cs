using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int RoleId { get; set; }

    public int? CountryId { get; set; }

    public int? StateId { get; set; }

    public int? CityId { get; set; }

    public string? Address { get; set; }

    public string? ZipCode { get; set; }

    public string? ProfileImage { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? HasLoggedInBefore { get; set; }

    public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; } = new List<Category>();

    public virtual ICollection<Category> CategoryUpdatedByNavigations { get; set; } = new List<Category>();

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<CustomerReview> CustomerReviewCreatedByNavigations { get; set; } = new List<CustomerReview>();

    public virtual ICollection<CustomerReview> CustomerReviewUpdatedByNavigations { get; set; } = new List<CustomerReview>();

    public virtual ICollection<Customer> CustomerUpdatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Item> ItemCreatedByNavigations { get; set; } = new List<Item>();

    public virtual ICollection<ItemModifiergroup> ItemModifiergroupCreatedByNavigations { get; set; } = new List<ItemModifiergroup>();

    public virtual ICollection<ItemModifiergroup> ItemModifiergroupUpdatedByNavigations { get; set; } = new List<ItemModifiergroup>();

    public virtual ICollection<Item> ItemUpdatedByNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Modifier> ModifierCreatedByNavigations { get; set; } = new List<Modifier>();

    public virtual ICollection<ModifierGroup> ModifierGroupCreatedByNavigations { get; set; } = new List<ModifierGroup>();

    public virtual ICollection<ModifierGroup> ModifierGroupUpdatedByNavigations { get; set; } = new List<ModifierGroup>();

    public virtual ICollection<Modifier> ModifierUpdatedByNavigations { get; set; } = new List<Modifier>();

    public virtual ICollection<Order> OrderCreatedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<OrderModifier> OrderModifierCreatedByNavigations { get; set; } = new List<OrderModifier>();

    public virtual ICollection<OrderModifier> OrderModifierUpdatedByNavigations { get; set; } = new List<OrderModifier>();

    public virtual ICollection<OrderTaxis> OrderTaxisCreatedByNavigations { get; set; } = new List<OrderTaxis>();

    public virtual ICollection<OrderTaxis> OrderTaxisUpdatedByNavigations { get; set; } = new List<OrderTaxis>();

    public virtual ICollection<Order> OrderUpdatedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Permission> PermissionCreatedByNavigations { get; set; } = new List<Permission>();

    public virtual ICollection<Permission> PermissionUpdatedByNavigations { get; set; } = new List<Permission>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissionCreatedByNavigations { get; set; } = new List<RolePermission>();

    public virtual ICollection<RolePermission> RolePermissionUpdatedByNavigations { get; set; } = new List<RolePermission>();

    public virtual ICollection<Section> SectionCreatedByNavigations { get; set; } = new List<Section>();

    public virtual ICollection<Section> SectionUpdatedByNavigations { get; set; } = new List<Section>();

    public virtual State? State { get; set; }

    public virtual ICollection<Table> TableCreatedByNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Table> TableUpdatedByNavigations { get; set; } = new List<Table>();

    public virtual User UpdatedByNavigation { get; set; } = null!;

    public virtual ICollection<WaitingList> WaitingListCreatedByNavigations { get; set; } = new List<WaitingList>();

    public virtual ICollection<WaitingList> WaitingListUpdatedByNavigations { get; set; } = new List<WaitingList>();
}
