using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class WallPostComments
{
    public int IdwallPostComments { get; set; }

    public string? CommentText { get; set; }

    public DateTime? InsertDateTime { get; set; }

    public int? MemberDetailsId { get; set; }

    public int? WallId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual MemberDetails? MemberDetails { get; set; }

    public virtual Wall? Wall { get; set; }
}
