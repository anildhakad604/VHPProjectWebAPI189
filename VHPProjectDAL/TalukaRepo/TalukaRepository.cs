using Microsoft.EntityFrameworkCore;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.TalukaRepo
{
    public class TalukaRepository : ITalukaRepository
    {
        private readonly MasterProjContext _context;
        public TalukaRepository(MasterProjContext context)
        {
            _context = context;

        }
        public async Task<IEnumerable<Talukamaster>> GetActiveTalukasAsync(string? talukaName)
        {
            var query = _context.Talukamaster
                 .Where(t => t.IsActive == true)
                 .AsQueryable();

            if (!string.IsNullOrEmpty(talukaName))
            {
                query = query.Where(t => EF.Functions.Like(t.TalukaName, $"%{talukaName}%"));
            }

            return await query.OrderBy(t => t.TalukaName).ToListAsync();
        }
    }
}
