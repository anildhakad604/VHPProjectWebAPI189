using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class CreateWallDto
    {
        public string PostMessages { get; set; }
        public int MemberId { get; set; }
        public byte Active { get; set; } = 1;
    }
}
