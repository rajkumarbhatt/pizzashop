using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int NumberOfPeople { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<CustomerReview> CustomerReviews { get; set; } = new List<CustomerReview>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User UpdatedByNavigation { get; set; } = null!;

    public virtual ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
}
