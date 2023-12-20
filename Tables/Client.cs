using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Client
{
    public int Idclient { get; set; }

    public int Iduser { get; set; }

    public string Phone { get; set; } = null!;

    public virtual User IduserNavigation { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
