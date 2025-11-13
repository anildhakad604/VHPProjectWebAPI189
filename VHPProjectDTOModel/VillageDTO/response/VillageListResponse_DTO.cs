using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.VillageDTO.response
{
    public class VillageListResponse_DTO
    {
        public List<VillageResponse_DTO> Villages { get; set; } = new();
    }
}
