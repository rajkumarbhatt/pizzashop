using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class City
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public string? Name { get; set; }

    public virtual State State { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
