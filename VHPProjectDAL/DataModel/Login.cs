using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Login
{
    public int IdLogin { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
