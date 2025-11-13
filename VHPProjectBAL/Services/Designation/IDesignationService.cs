using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDTOModel.DesignationDTO.request;
using VHPProjectDTOModel.DesignationDTO.response;

namespace VHPProjectBAL.Services.Designation
{
    public interface IDesignationService
    {
        Task<DesignationListResponse_DTO> GetActiveDesignationsAsync();
        //Task<DesignationResponse_DTO?> GetByIdAsync(int id);
        //Task AddDesignationAsync(AddDesignationRequest_DTO request);
        //Task UpdateDesignationAsync(UpdateDesignationRequest_DTO request);
        //Task DeleteDesignationAsync(int id);
    }
}
