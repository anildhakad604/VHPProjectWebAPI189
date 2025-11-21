using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.VillageRepo
{
    public interface IVillageRepository
    {
        Task<IEnumerable<Villagemaster>> GetActiveVillagesAsync(int? talukaMasterId, string? villageName);
    }
}
