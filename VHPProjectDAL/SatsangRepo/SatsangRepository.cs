using Microsoft.EntityFrameworkCore;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.SatsangRepo
{
    public class SatsangRepository : ISatsangRepository
    {
        private readonly MasterProjContext _context;
        private readonly ILoggerManager _logger;

        public SatsangRepository(MasterProjContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }



        public async Task<int> AddSatsangAsync(Satsang satsang)
        {
            try
            {
                // Prevent duplicate for same temple and date range
                bool exists = await _context.Satsang.AnyAsync(s =>
                    s.TempleName == satsang.TempleName &&
                    s.FromDate <= satsang.ToDate && s.ToDate >= satsang.FromDate);

                if (exists)
                    throw new Exception("A satsang already exists for this temple within the given date range.");

                await _context.Satsang.AddAsync(satsang);
                await _context.SaveChangesAsync();
                return satsang.IdSatsang;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding satsang",ex);
                throw;
            }
        }

        public async Task<(IEnumerable<Satsang>, int totalCount)> GetSatsangDetailsAsync(int? villageMasterId, int? talukaMasterId, string? templeName, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Satsang
                    .Include(x => x.TalukaMaster)
                    .Include(x => x.VillageMaster)
                    .AsQueryable();

                if (villageMasterId.HasValue)
                    query = query.Where(x => x.VillageMasterId == villageMasterId);
                if (talukaMasterId.HasValue)
                    query = query.Where(x => x.TalukaMasterId == talukaMasterId);
                if (!string.IsNullOrEmpty(templeName))
                    query = query.Where(x => x.TempleName.Contains(templeName));
                if (fromDate.HasValue)
                    query = query.Where(x => x.FromDate >= fromDate);
                if (toDate.HasValue)
                    query = query.Where(x => x.ToDate <= toDate);

                int totalCount = await query.CountAsync();
                var satsangs = await query
                    .OrderByDescending(x => x.FromDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (satsangs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching satsang details",ex);
                throw;

            }
        }
    }
}
