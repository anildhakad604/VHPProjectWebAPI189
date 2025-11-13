using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.Repository.SatsangRepo
{
    public interface ISatsangRepository
    {
        Task<int> AddSatsangAsync(Satsang satsang);
        Task<(IEnumerable<Satsang>, int totalCount)> GetSatsangDetailsAsync(
            int? villageMasterId,
            int? talukaMasterId,
            string? templeName,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize);
    }
}
