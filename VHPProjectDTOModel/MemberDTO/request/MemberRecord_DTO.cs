using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class MemberRecord_DTO
    {
        public int MemberId { get; set; }
        public string? MobileNumber { get; set; }
        public int Is_Active { get; set; }
        public int Is_Admin { get; set; }
        public int Is_Core { get; set; }
        public string VillageName { get; set; }
        public int? Village_Master_Id { get; set; }
        public int? Taluka_Master_Id { get; set; }
        public int? Designation_Id { get; set; }
    }
}
