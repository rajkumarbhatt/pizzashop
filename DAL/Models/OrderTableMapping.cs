using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderTableMapping
{
    public int Id { get; set; }

    public int? Orderid { get; set; }

    public int? Tableid { get; set; }

    public short Noofpersons { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public string? Updatedby { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Table? Table { get; set; }
}
