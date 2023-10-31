using System;
using System.Collections.Generic;

namespace KPO_Cursovaya.Models;

public partial class Vkuser
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual User? User { get; set; }
}
