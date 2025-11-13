using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Refreshtoken
{
    public int IdRefreshToken { get; set; }

    public string Value { get; set; } = null!;

    public bool? IsUsed { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime ExpiryDate { get; set; }
}
