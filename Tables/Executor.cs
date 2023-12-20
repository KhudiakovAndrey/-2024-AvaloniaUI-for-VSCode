using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Executor
{
    public int Idexecutor { get; set; }

    public int Iduser { get; set; }

    public int IdtypeProblem { get; set; }

    public virtual TypeProblem IdtypeProblemNavigation { get; set; } = null!;

    public virtual User IduserNavigation { get; set; } = null!;

    public virtual ICollection<ReleaseRequest> ReleaseRequests { get; } = new List<ReleaseRequest>();

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
