using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.Repository.VillageRepo
{
    public class VillageRepository : IVillageRepository
    {
        private readonly MasterProjContext _context;
        public VillageRepository(MasterProjContext context)
        {
            _context=context;
        }
        public async Task<IEnumerable<object>> GetActiveVillagesAsync(int? talukaMasterId, string? villageName)
        {
            var query = _context.Villagemaster
                .Include(v => v.TalukaMaster)
                .Where(v => v.IsActive == true)
                .AsQueryable();

            if (talukaMasterId.HasValue)
            {
                query = query.Where(v => v.TalukaMasterId == talukaMasterId);
            }

            if (!string.IsNullOrEmpty(villageName))
            {
                query = query.Where(v => EF.Functions.Like(v.VillageName, $"%{villageName}%"));
            }

            return await query
                .OrderBy(v => v.VillageName)
                .Select(v => new
                {
                    v.VillageMasterId,
                    v.VillageName,
                    v.TalukaMasterId,
                    TalukaName = v.TalukaMaster.TalukaName
                })
                .ToListAsync();
        }
    }
}
