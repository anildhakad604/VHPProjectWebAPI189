using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Userotp
{
    public int Id { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string OtpCode { get; set; } = null!;

    public DateTime ExpireAt { get; set; }

    public bool IsVerified { get; set; }
}
