using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class TypeProblem
{
    public int IdtypeProblem { get; set; }

    public string Title { get; set; } = null!;

    public decimal Cost { get; set; }

    public int RepairTime { get; set; }

    public virtual ICollection<Equipment> Equipment { get; } = new List<Equipment>();

    public virtual ICollection<Executor> Executors { get; } = new List<Executor>();
}
