using Microsoft.AspNetCore.Http;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class MemberUploadRequestDTO
    {
        public IFormFile File { get; set; }
        public string PathName { get; set; }
    }
}
