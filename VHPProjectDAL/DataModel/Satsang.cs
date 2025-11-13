using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Satsang
{
    public int IdSatsang { get; set; }

    public string SatsangName { get; set; } = null!;

    public string TempleName { get; set; } = null!;

    public string TempleAddress { get; set; } = null!;

    public int TalukaMasterId { get; set; }

    public int VillageMasterId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual Talukamaster TalukaMaster { get; set; } = null!;

    public virtual Villagemaster VillageMaster { get; set; } = null!;
}
