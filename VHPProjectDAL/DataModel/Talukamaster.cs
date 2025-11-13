using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Talukamaster
{
    public int TalukaMasterId { get; set; }

    public string TalukaName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Member> Member { get; set; } = new List<Member>();

    public virtual ICollection<Satsang> Satsang { get; set; } = new List<Satsang>();

    public virtual ICollection<Villagemaster> Villagemaster { get; set; } = new List<Villagemaster>();
}
