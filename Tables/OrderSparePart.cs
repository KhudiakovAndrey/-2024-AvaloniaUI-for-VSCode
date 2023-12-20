using System;
using System.Collections.Generic;

namespace RequestServices_Ivanov;

public partial class OrderSparePart
{
    public int IdorderSparePart { get; set; }

    public int IdsparePart { get; set; }

    public int Count { get; set; }

    public int Idequipment { get; set; }

    public virtual Equipment IdequipmentNavigation { get; set; } = null!;

    public virtual SparePart IdsparePartNavigation { get; set; } = null!;
}
