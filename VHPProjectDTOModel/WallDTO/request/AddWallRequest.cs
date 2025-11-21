using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.WallDTO.request
{
    public class AddWallRequest
    {
        public AddWallDto AddWall { get; set; } = new();
        public List<AddPostImageDto>? AddPostImages { get; set; }
    }

    public class AddWallDto
    {
        public string? PostMessage { get; set; }
        public int MemberDetailsId { get; set; }
    }

    public class AddPostImageDto
    {
        public string? ImageUrl { get; set; }
    }
}
