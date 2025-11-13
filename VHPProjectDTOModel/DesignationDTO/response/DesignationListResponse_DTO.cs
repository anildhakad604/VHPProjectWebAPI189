using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.DesignationDTO.response
{
    public class DesignationListResponse_DTO
    {
        public List<DesignationResponse_DTO> Designations { get; set; } = new();
    }
}
