using VHPProjectBAL.Services.VillageMaster;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.VillageRepo;
using VHPProjectDTOModel.VillageDTO.request;
using VHPProjectDTOModel.VillageDTO.response;

namespace VHPProjectBAL.Services.ViilageMaster
{
    public class VillageService : IVillageService
    {
        private readonly IVillageRepository _villageRepository;
        private readonly ILoggerManager _logger;

        public VillageService(IVillageRepository villageRepository, ILoggerManager logger)
        {
            _villageRepository = villageRepository;
            _logger = logger;
        }

        public async Task<ResultWithDataDTO<VillageListResponse_DTO>> GetActiveVillagesAsync(VillageListRequest_DTO request)
        {
            var result = new ResultWithDataDTO<VillageListResponse_DTO>();

            try
            {
                _logger.LogInfo("VillageService => GetActiveVillagesAsync invoked.");

                // Basic Validation
                if (request == null)
                {
                    result.IsSuccessful = false;
                    result.IsBusinessError = true;
                    result.BusinessErrorMessage = "Invalid request.";
                    result.Message = "Request cannot be null.";

                    _logger.LogWarn("VillageService => GetActiveVillagesAsync request is null.");
                    return result;
                }

                // Fetch data from Repository
                var data = await _villageRepository.GetActiveVillagesAsync(
                    request.TalukaMasterId,
                    request.VillageName
                );

                // Map to DTO
                var villageList = data.Select(v => new VillageResponse_DTO
                {
                    VillageMasterId = v.VillageMasterId,
                    VillageName = v.VillageName,
                    TalukaMasterId = v.TalukaMasterId
                    
                }).ToList();

                result.Data = new VillageListResponse_DTO
                {
                    Villages = villageList
                };

                result.IsSuccessful = true;
                result.Message = "Village data fetched successfully.";

                _logger.LogInfo("VillageService => GetActiveVillagesAsync executed successfully.");
            }
            catch (Exception ex)
            {
                // Log Error
                _logger.LogError($"Error in VillageService => GetActiveVillagesAsync: {ex.Message}", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error fetching village data.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
