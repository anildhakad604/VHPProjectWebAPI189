using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.Repository.DesignationRepo
{
    public interface IDesignationRepository
    {
        Task<IEnumerable<Designation>> GetActiveDesignationAsync();
        //Task<Designation?> GetByIdAsync(int id);
        //Task AddAsync(Designation designation);
        //Task UpdateAsync(Designation designation);
        //Task DeleteAsync(int id);

    }
}
