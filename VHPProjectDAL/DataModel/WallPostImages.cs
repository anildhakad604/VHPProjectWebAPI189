using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class WallPostImages
{
    public int IdwallPostImages { get; set; }

    public string? ImageUrl { get; set; }

    public int? WallId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ulong? Active { get; set; }

    public virtual Wall? Wall { get; set; }
}
