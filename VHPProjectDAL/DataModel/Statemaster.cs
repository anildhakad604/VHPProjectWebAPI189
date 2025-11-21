using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Statemaster
{
    public int StateId { get; set; }

    public string StateName { get; set; } = null!;

    public virtual ICollection<Talukamaster> Talukamaster { get; set; } = new List<Talukamaster>();
}
