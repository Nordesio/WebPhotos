using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KPO_Cursovaya.Models;

public partial class Vkuser
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    [Display(Name = "Ссылка")]
    public string Url { get; set; } = null!;
    [Display(Name = "Дата")]
    public DateOnly? Date { get; set; }
    [Display(Name = "Статус")]
    public string? Status { get; set; }
    [Display(Name = "Название")]
    public string? Name { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual User? User { get; set; }
}
