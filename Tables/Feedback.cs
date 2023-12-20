using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Feedback
{
    public int Idrequest { get; set; }

    public string Comment { get; set; } = null!;

    public virtual Request IdrequestNavigation { get; set; } = null!;
}
