using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Role
{
    public int Idrole { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
