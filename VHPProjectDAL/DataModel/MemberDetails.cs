using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class MemberDetails
{
    public int IdMember { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MobileNumber { get; set; }

    public string? EmailId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual ICollection<Wall> Wall { get; set; } = new List<Wall>();

    public virtual ICollection<WallPostComments> WallPostComments { get; set; } = new List<WallPostComments>();

    public virtual ICollection<WallPostLike> WallPostLike { get; set; } = new List<WallPostLike>();
}
