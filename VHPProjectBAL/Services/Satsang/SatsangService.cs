using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.DataModel;
using VHPProjectDAL.Repository.SatsangRepo;
using VHPProjectDTOModel.SatsangDTO.request;
using VHPProjectDTOModel.SatsangDTO.response;

namespace VHPProjectBAL.Services.SatsangService
{
    public class SatsangService:ISatsangService
    {
        private readonly ISatsangRepository _satsangRepository;
        private readonly ILoggerManager _logger;

        public SatsangService(ISatsangRepository satsangRepository, ILoggerManager logger)
        {
            _satsangRepository = satsangRepository;
            _logger = logger;
        }

        public async Task<ResultWithDataDTO<int>> AddSatsangAsync(AddSatsangRequest_DTO request)
        {
            var result = new ResultWithDataDTO<int>();

            try
            {
                if (string.IsNullOrWhiteSpace(request.SatsangName) ||
                    string.IsNullOrWhiteSpace(request.TempleName) ||
                    string.IsNullOrWhiteSpace(request.TempleAddress))
                {
                    result.IsSuccessful = false;
                    result.Message = "Required fields are missing.";
                    return result;
                }

                if (request.FromDate >= request.ToDate)
                {
                    result.IsSuccessful = false;
                    result.Message = "FromDate must be earlier than ToDate.";
                    return result;
                }

                var entity = new Satsang
                {
                    SatsangName = request.SatsangName,
                    TempleName = request.TempleName,
                    TempleAddress = request.TempleAddress,
                    TalukaMasterId = request.TalukaMasterId,
                    VillageMasterId = request.VillageMasterId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    CreatedDate = DateTime.Now,
                    Active = 1
                };

                var id = await _satsangRepository.AddSatsangAsync(entity);

                result.IsSuccessful = true;
                result.Data = id;
                result.Message = "Satsang data added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while adding satsang", ex);
                result.IsSuccessful = false;
                result.Message = "Error while adding satsang.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<ResultWithDataDTO<SatsangListResponse_DTO>> GetSatsangDetailsAsync(
            int? villageMasterId, int? talukaMasterId, string? templeName,
            DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var result = new ResultWithDataDTO<SatsangListResponse_DTO>();

            try
            {
                var (entities, totalCount) = await _satsangRepository.GetSatsangDetailsAsync(
                    villageMasterId, talukaMasterId, templeName, fromDate, toDate, pageNumber, pageSize);

                var list = entities.Select(s => new SatsangResponse_DTO
                {
                    IdSatsang = s.IdSatsang,
                    SatsangName = s.SatsangName,
                    TempleName = s.TempleName,
                    TempleAddress = s.TempleAddress,
                    TalukaMasterId = s.TalukaMasterId,
                    TalukaName = s.TalukaMaster?.TalukaName,
                    VillageMasterId = s.VillageMasterId,
                    VillageName = s.VillageMaster?.VillageName,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate
                }).ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                result.IsSuccessful = true;
                result.Data = new SatsangListResponse_DTO
                {
                    Satsangs = list,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    IsNextPage = pageNumber < totalPages,
                    IsPrevPage = pageNumber > 1,
                    CurrentPage = pageNumber
                };
                result.Message = "Satsang data retrieved successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching satsang details", ex);
                result.IsSuccessful = false;
                result.Message = "Error while fetching satsang details.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }

    }
}
