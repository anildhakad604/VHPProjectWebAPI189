using Microsoft.EntityFrameworkCore;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.VillageRepo
{
    public class VillageRepository : IVillageRepository
    {
        private readonly MasterProjContext _context;
        public VillageRepository(MasterProjContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Villagemaster>> GetActiveVillagesAsync(int? talukaMasterId, string? villageName)
        {
            var result = await(from v in _context.Villagemaster
                               join t in _context.Talukamaster on v.TalukaMasterId equals t.TalukaMasterId
                               where v.IsActive == true
                               && (talukaMasterId == 0 || v.TalukaMasterId == talukaMasterId)
                                     && (string.IsNullOrEmpty(villageName) || v.VillageName.Contains(villageName))
                               select new Villagemaster
                               {
                                   VillageMasterId = v.VillageMasterId,
                                   VillageName = v.VillageName,
                                   TalukaMasterId = v.TalukaMasterId,
                                   
                               }).ToListAsync();

            return result;
        }
    }
    
}
