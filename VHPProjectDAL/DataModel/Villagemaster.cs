using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Villagemaster
{
    public int VillageMasterId { get; set; }

    public string VillageName { get; set; } = null!;

    public int TalukaMasterId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Member> Member { get; set; } = new List<Member>();

    public virtual ICollection<Satsang> Satsang { get; set; } = new List<Satsang>();

    public virtual Talukamaster TalukaMaster { get; set; } = null!;
}
