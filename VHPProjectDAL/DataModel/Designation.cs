using System;
using System.Collections.Generic;

namespace VHPProjectDAL.DataModel;

public partial class Designation
{
    public int DesignationId { get; set; }

    public string DesignationName { get; set; } = null!;

    public bool? IsActive { get; set; }
}
