using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDAL.Repository.VillageRepo
{
    public interface IVillageRepository
    {
        Task<IEnumerable<object>> GetActiveVillagesAsync(int? talukaMasterId, string? villageName);
    }
}
