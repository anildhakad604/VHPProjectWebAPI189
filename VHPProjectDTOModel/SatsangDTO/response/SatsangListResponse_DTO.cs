using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.SatsangDTO.response
{
    public class SatsangListResponse_DTO
    {
        public List<SatsangResponse_DTO> Satsangs { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool IsNextPage { get; set; }
        public bool IsPrevPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
