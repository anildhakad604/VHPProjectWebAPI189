using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.DesignationDTO.request;
using VHPProjectDTOModel.DesignationDTO.response;

namespace VHPProjectBAL.Services.Designation
{
    public interface IDesignationService
    {
        Task<ResultWithDataDTO<IEnumerable<DesignationResponse_DTO>>> GetActiveAsync();

        Task<ResultWithDataDTO<DesignationResponse_DTO?>> GetByIdAsync(int id);

        Task<ResultWithDataDTO<string>> AddDesignationAsync(AddDesignationRequest_DTO request);

        Task<ResultWithDataDTO<string>> UpdateDesignationAsync(UpdateDesignationRequest_DTO request);

        Task<ResultWithDataDTO<string>> DeleteDesignationAsync(int id);




    }
}
