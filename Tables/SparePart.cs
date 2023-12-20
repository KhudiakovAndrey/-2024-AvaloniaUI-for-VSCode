using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class SparePart
{
    public int IdsparePart { get; set; }

    public string Title { get; set; } = null!;

    public decimal Cost { get; set; }

    public DateTime? DeliveryTime { get; set; }

    public virtual ICollection<OrderSparePart> OrderSpareParts { get; } = new List<OrderSparePart>();
}
