using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.SatsangDTO.request;
using VHPProjectDTOModel.SatsangDTO.response;

namespace VHPProjectBAL.Services.SatsangService
{
    public interface ISatsangService
    {
        Task<ResultWithDataDTO<int>> AddSatsangAsync(AddSatsangRequest_DTO request);

        Task<ResultWithDataDTO<SatsangListResponse_DTO>> GetSatsangDetailsAsync(int? villageMasterId,
            int? talukaMasterId,
            string? templeName,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize);
    }
}
