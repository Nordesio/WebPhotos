using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DB.Models;

public class User
{
    public string Id { get; set; } = null!;
    [DisplayName("Фамилия")]
    public string? Surname { get; set; }
    [DisplayName("Имя")]
    public string? Name { get; set; }
    [DisplayName("Отчество")]
    public string? Patronymic { get; set; }
    //[DataType(DataType.Date)]
    //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    [DisplayName("День рождения")]
    public DateOnly? Birthday { get; set; }
    [DisplayName("Электронная почта")]
    public string? Email { get; set; }
    [DisplayName("Почта подтверждена")]
    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }
    [DisplayName("Номер телефона")]
    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }
    //[DataType(DataType.Date)]
    //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateOnly? LockoutEndDateUtc { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }
    [DisplayName("Никнейм")]
    public string UserName { get; set; } = null!;

    public int? UserGroupId { get; set; }

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    public virtual UserGroup? UserGroup { get; set; }

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
