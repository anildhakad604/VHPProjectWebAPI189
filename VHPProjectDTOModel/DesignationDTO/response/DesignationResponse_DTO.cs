using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.DesignationDTO.response
{
    public class DesignationResponse_DTO
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
