using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.WallDTO.request
{
    public class AddPostCommentRequest
    {
        public string? Comment { get; set; }
        public int MemberDetailsId { get; set; }
        public int WallId { get; set; }
    }
}
