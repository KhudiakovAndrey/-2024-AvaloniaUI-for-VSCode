using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class Equipment
{
    public int Idequipment { get; set; }

    public string TypeEquipment { get; set; } = null!;

    public string DescriptionProblem { get; set; } = null!;

    public int IdtypeProblem { get; set; }

    public string Title { get; set; } = null!;

    public virtual TypeProblem IdtypeProblemNavigation { get; set; } = null!;

    public virtual ICollection<OrderSparePart> OrderSpareParts { get; } = new List<OrderSparePart>();

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
