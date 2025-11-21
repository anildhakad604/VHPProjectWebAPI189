using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class WallPostLike
{
    public int IdwallPostLike { get; set; }

    public int? MemberDetailsId { get; set; }

    public int? WallId { get; set; }

    public ulong? Like { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual MemberDetails? MemberDetails { get; set; }

    public virtual Wall? Wall { get; set; }
}
