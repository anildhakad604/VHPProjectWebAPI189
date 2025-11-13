using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.VillageDTO.request;
using VHPProjectDTOModel.VillageDTO.response;

namespace VHPProjectBAL.Services.VillageMaster
{
    public interface IVillageService
    {
        Task<ResultWithDataDTO<VillageListResponse_DTO>> GetActiveVillagesAsync(VillageListRequest_DTO request);
    }
}
