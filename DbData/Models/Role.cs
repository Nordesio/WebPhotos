using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbData.Models;

public partial class Role
{
    [Display(Name = "Роль")]
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
