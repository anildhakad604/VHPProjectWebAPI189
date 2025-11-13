using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Member
{
    public int MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? VillageMasterId { get; set; }

    public int? TalukaMasterId { get; set; }

    public bool IsAdmin { get; set; }

    public virtual Talukamaster? TalukaMaster { get; set; }

    public virtual Villagemaster? VillageMaster { get; set; }
}
