using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectBAL.Services.VillageMaster;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.Repository.VillageRepo;
using VHPProjectDTOModel.VillageDTO.request;
using VHPProjectDTOModel.VillageDTO.response;

namespace VHPProjectBAL.Services.ViilageMaster
{
    public class VillageService:IVillageService
    {
        private readonly IVillageRepository _villageRepository;

        public VillageService(IVillageRepository villageRepository)
        {
            _villageRepository = villageRepository;
        }

        public async Task<ResultWithDataDTO<VillageListResponse_DTO>> GetActiveVillagesAsync(VillageListRequest_DTO request)
        {
            var result = new ResultWithDataDTO<VillageListResponse_DTO>();

            try
            {
                var data = await _villageRepository.GetActiveVillagesAsync(request.TalukaMasterId, request.VillageName);

                var dto = new VillageListResponse_DTO
                {
                    Villages = data.Select(v => new VillageResponse_DTO
                    {
                        VillageMasterId = (int)v.GetType().GetProperty("VillageMasterId")?.GetValue(v)!,
                        VillageName = v.GetType().GetProperty("VillageName")?.GetValue(v)?.ToString() ?? string.Empty,
                        TalukaMasterId = (int)v.GetType().GetProperty("TalukaMasterId")?.GetValue(v)!,
                        TalukaName = v.GetType().GetProperty("TalukaName")?.GetValue(v)?.ToString() ?? string.Empty
                    }).ToList()
                };

                result.Data = dto;
                result.IsSuccessful = true;
                result.Message = "Village data fetched successfully.";
            }
            catch (System.Exception ex)
            {
                result.IsSuccessful = false;
                result.Message = $"Error fetching village data: {ex.Message}";
            }

            return result;
        }
    }
}
