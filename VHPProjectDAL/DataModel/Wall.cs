using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Wall
{
    public int IdWall { get; set; }

    public string? PostMessages { get; set; }

    public int? MemberDetailsId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual MemberDetails? MemberDetails { get; set; }

    public virtual ICollection<WallPostComments> WallPostComments { get; set; } = new List<WallPostComments>();

    public virtual ICollection<WallPostImages> WallPostImages { get; set; } = new List<WallPostImages>();

    public virtual ICollection<WallPostLike> WallPostLike { get; set; } = new List<WallPostLike>();
}
