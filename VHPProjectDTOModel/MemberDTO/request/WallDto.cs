using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class WallDto
    {
        public int Id { get; set; }
        public string PostMessages { get; set; }
        public int MemberId { get; set; }
        public byte Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
