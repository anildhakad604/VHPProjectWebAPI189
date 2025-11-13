using VHPProjectCommonUtility.Response;
using VHPProjectDAL.Repository.TalukaRepo;
using VHPProjectDTOModel.TalukaDTO.request;
using VHPProjectDTOModel.TalukaDTO.response;

namespace VHPProjectBAL.Services.TalukaMaster
{
    public class TalukaService : ITalukaService
    {
        private readonly ITalukaRepository _talukaRepository;

        public TalukaService(ITalukaRepository talukaRepository)
        {
            _talukaRepository = talukaRepository;
            
        }

        public async Task<ResultWithDataDTO<TalukaListResponse_DTO>> GetActiveTalukasAsync(TalukaListRequest_DTO request)
        {
            var result = new ResultWithDataDTO<TalukaListResponse_DTO>();

            try
            {
                var talukas = await _talukaRepository.GetActiveTalukasAsync(request.TalukaName);

                result.Data = new TalukaListResponse_DTO
                {
                    Talukas = talukas.Select(t => new TalukaResponse_DTO
                    {
                        TalukaMasterId = t.TalukaMasterId,
                        TalukaName = t.TalukaName
                    }).ToList()
                };

                result.IsSuccessful = true;
                result.Message = "Taluka data fetched successfully.";
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Message = $"Error fetching taluka data: {ex.Message}";
            }

            return result;
        }
    }
}
