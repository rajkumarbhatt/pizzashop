using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ResetPasswordLink
{
    public string Link { get; set; } = null!;

    public short Id { get; set; }
}
