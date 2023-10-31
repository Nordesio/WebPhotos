using System;
using System.Collections.Generic;

namespace KPO_Cursovaya.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Vkuser> Vkusers { get; set; } = new List<Vkuser>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
