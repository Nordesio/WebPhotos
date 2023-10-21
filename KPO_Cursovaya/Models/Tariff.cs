using System;
using System.Collections.Generic;

namespace DB.Models;

public partial class Tariff
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string QueryCount { get; set; } = null!;

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
