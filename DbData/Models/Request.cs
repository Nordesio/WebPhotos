﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbData.Models;

public partial class Request
{
    public int Id { get; set; }

    public int VkuserId { get; set; }
    [Display(Name = "Дата")]
    public DateOnly? Date { get; set; }
    [Display(Name = "Текст")]
    public string? Text { get; set; }
    [Display(Name = "Ссылка")]
    public string? Url { get; set; }
    [Display(Name = "Автор")]
    public string? Author { get; set; }

    public string? AuthorId { get; set; }

    public string? AuthorLink { get; set; }

    public byte[]? ImageByte { get; set; }

    public string? ImageLink { get; set; }

    public virtual ICollection<Entity> Entities { get; set; } = new List<Entity>();

    public virtual Vkuser Vkuser { get; set; } = null!;
}
