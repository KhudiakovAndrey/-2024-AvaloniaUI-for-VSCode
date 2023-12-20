using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class User
{
    public int Iduser { get; set; }

    public string Fio { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Idrole { get; set; }

    public virtual ICollection<Client> Clients { get; } = new List<Client>();

    public virtual ICollection<Executor> Executors { get; } = new List<Executor>();

    public virtual Role IdroleNavigation { get; set; } = null!;
}
