using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.VillageDTO.response
{
    public class VillageResponse_DTO
    {
        public int VillageMasterId { get; set; }
        public string VillageName { get; set; } = string.Empty;
        public int TalukaMasterId { get; set; }
        public string TalukaName { get; set; } = string.Empty;
    }
}
