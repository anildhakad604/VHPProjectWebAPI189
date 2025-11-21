using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.responses
{
    public class FileUploadResponseDTO
    {
        public List<string> FilePaths { get; set; } = new();
    }
}
