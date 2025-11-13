using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class UpdateMemberRequestDTO:AddMemberRequestDTO
    {
        [Required(ErrorMessage = "MemberId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "MemberId must be a positive integer")]
        public int MemberId { get; set; }
    }
}
