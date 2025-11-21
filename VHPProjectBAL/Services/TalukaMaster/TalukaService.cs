using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.TalukaRepo;
using VHPProjectDTOModel.TalukaDTO.request;
using VHPProjectDTOModel.TalukaDTO.response;

namespace VHPProjectBAL.Services.TalukaMaster
{
    public class TalukaService : ITalukaService
    {
        private readonly ITalukaRepository _talukaRepository;
        private readonly ILoggerManager _logger;

        public TalukaService(ITalukaRepository talukaRepository, ILoggerManager logger)
        {
            _talukaRepository = talukaRepository;
            _logger = logger;
        }

        public async Task<ResultWithDataDTO<TalukaListResponse_DTO>> GetActiveTalukasAsync(TalukaListRequest_DTO request)
        {
            var result = new ResultWithDataDTO<TalukaListResponse_DTO>();

            try
            {
               
                _logger.LogInfo("TalukaService => GetActiveTalukasAsync called");

               
                var talukas = await _talukaRepository.GetActiveTalukasAsync(request.TalukaName);

                
                var list = talukas.Select(t => new TalukaResponse_DTO
                {
                    TalukaMasterId = t.TalukaMasterId,
                    TalukaName = t.TalukaName
                }).ToList();

              
                result.Data = new TalukaListResponse_DTO
                {
                    Talukas = list
                };

                result.IsSuccessful = true;
                result.Message = "Taluka data fetched successfully.";

                _logger.LogInfo("TalukaService => GetActiveTalukasAsync completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in TalukaService => GetActiveTalukasAsync", ex);

                result.IsSuccessful = false;
                result.Message = "Error fetching taluka data.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
