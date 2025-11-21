using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class FileDeleteRequestDTO
    {
        public List<string> FilePath { get; set; } = new();
    }
}
