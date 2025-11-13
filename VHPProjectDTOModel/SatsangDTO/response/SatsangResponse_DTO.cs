using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.SatsangDTO.response
{
    public class SatsangResponse_DTO
    {
        public int IdSatsang { get; set; }
        public string SatsangName { get; set; }
        public string TempleName { get; set; }
        public string TempleAddress { get; set; }
        public int TalukaMasterId { get; set; }
        public string TalukaName { get; set; }
        public int VillageMasterId { get; set; }
        public string VillageName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
