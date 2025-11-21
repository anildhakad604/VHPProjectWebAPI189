using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.WallDTO.request
{
    public class WallDataFlat
    {
        public int IdWall { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string PostMessage { get; set; }
        public DateTime WallDate { get; set; }
        public string ImageUrls { get; set; }         // comma-separated
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        public int? CommentId { get; set; }
        public string CommentText { get; set; }
        public int? CommentMemberId { get; set; }
        public string CommentMemberName { get; set; }
        public DateTime? CommentDate { get; set; }
    }

}
