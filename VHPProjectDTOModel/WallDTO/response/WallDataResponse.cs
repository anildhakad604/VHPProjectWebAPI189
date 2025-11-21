using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.WallDTO.response
{
    public class WallDataResponse
    {
        public int IdWall { get; set; }
        public int MemberDetailsId { get; set; }
        public string? MemberName { get; set; }
        public string? PostMessage { get; set; }
        public int IsActive { get; set; }
        public DateTime WallDate { get; set; }
        public List<CommentDto>? Comment { get; set; }
        public int Like { get; set; } // for current user's like status if needed
        public List<string>? ImageUrl { get; set; }
        public LikeCountDto? LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class CommentDto
    {
        public int IdWallPostComments { get; set; }
        public string? CommentString { get; set; }
        public int CommentMemberId { get; set; }
        public string? CommentMemberName { get; set; }
        public DateTime CommentDate { get; set; }
    }

    public class LikeCountDto
    {
        public int Count { get; set; }
    }
}
