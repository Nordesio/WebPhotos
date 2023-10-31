using System;
using System.Collections.Generic;

namespace KPO_Cursovaya.Models;

public partial class Request
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public DateOnly? Date { get; set; }

    public string? Text { get; set; }

    public string? Url { get; set; }

    public string? Author { get; set; }

    public string? AuthorId { get; set; }

    public string? AuthorLink { get; set; }

    public virtual Vkuser RequestNavigation { get; set; } = null!;
}
