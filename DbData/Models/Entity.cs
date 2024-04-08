using System;
using System.Collections.Generic;

namespace DbData.Models;

public partial class Entity
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public string? Geo { get; set; }

    public string? Date { get; set; }

    public string? Organization { get; set; }

    public string? Namedentity { get; set; }

    public string? Person { get; set; }

    public string? Street { get; set; }

    public string? Money { get; set; }

    public string? Address { get; set; }

    public virtual Request Request { get; set; } = null!;
}
