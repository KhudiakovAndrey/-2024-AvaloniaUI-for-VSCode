using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Request
{
    public int Idrequest { get; set; }

    public DateTime DateAdd { get; set; }

    public string? Priority { get; set; }

    public int Idequipment { get; set; }

    public int Idclient { get; set; }

    public int Idexecutor { get; set; }

    public string Status { get; set; } = null!;

    public string? Comment { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Client IdclientNavigation { get; set; } = null!;

    public virtual Equipment IdequipmentNavigation { get; set; } = null!;

    public virtual Executor IdexecutorNavigation { get; set; } = null!;

    public virtual ICollection<ReleaseRequest> ReleaseRequests { get; } = new List<ReleaseRequest>();
}
