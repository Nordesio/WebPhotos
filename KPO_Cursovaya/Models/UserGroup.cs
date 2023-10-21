using System;
using System.Collections.Generic;

namespace DB.Models;

public partial class UserGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly FinishDate { get; set; }

    public bool Subscription { get; set; }

    public int? TariffId { get; set; }

    public string? TelegramAccessKey { get; set; }

    public virtual Tariff? Tariff { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
