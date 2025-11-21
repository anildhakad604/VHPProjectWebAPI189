using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.SatsangRepo
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
