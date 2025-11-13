using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.SatsangDTO.request
{
    public class AddSatsangRequest_DTO
    {
        public string SatsangName { get; set; } = string.Empty;
        public string TempleName { get; set; } = string.Empty;
        public string TempleAddress { get; set; } = string.Empty;
        public int TalukaMasterId { get; set; }
        public int VillageMasterId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
