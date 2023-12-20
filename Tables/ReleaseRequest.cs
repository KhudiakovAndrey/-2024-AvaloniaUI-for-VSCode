using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class ReleaseRequest
{
    public int IdreleaseRequests { get; set; }

    public int Idexecutor { get; set; }

    public int Idrequest { get; set; }

    public virtual Executor IdexecutorNavigation { get; set; } = null!;

    public virtual Request IdrequestNavigation { get; set; } = null!;
}
