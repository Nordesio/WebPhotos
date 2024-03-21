using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace DbData.Models;

public partial class User
{
    public int Id { get; set; }
    [Display(Name = "Имя")]
    public string Name { get; set; } = null!;
    [Display(Name = "Почта")]
    public string? Email { get; set; }
    [Display(Name = "Подтверждена ли почта")]
    public bool EmailConfirmed { get; set; }
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;
    [Display(Name = "Роль")]
    public string? Role { get; set; }

    public virtual Role? RoleNavigation { get; set; }

    public virtual ICollection<Vkuser> Vkusers { get; set; } = new List<Vkuser>();
}
