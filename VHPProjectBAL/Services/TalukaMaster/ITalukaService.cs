using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.TalukaDTO.request;
using VHPProjectDTOModel.TalukaDTO.response;

namespace VHPProjectBAL.Services.TalukaMaster
{
    public interface ITalukaService
    {
        Task<ResultWithDataDTO<TalukaListResponse_DTO>> GetActiveTalukasAsync(TalukaListRequest_DTO request);
    }
}
