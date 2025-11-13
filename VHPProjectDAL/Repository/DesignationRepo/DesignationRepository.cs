using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.Repository.DesignationRepo
{
    public class DesignationRepository:IDesignationRepository
    {
        private readonly MasterProjContext _context;

        private readonly ILoggerManager _logger;

        public DesignationRepository(MasterProjContext context,ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Designation>> GetActiveDesignationAsync()
        {
           

            return await _context.Designation
                .Where(d => d.IsActive == true)
                .OrderBy(d => d.DesignationName)
                .ToListAsync();
           
        }

        //public async Task<Designation?> GetByIdAsync(int id)
        //{
        //    return await _context.Designation.FindAsync(id);
        //}

        //public async Task AddAsync(Designation designation)
        //{
        //    await _context.Designation.AddAsync(designation);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(Designation designation)
        //{
        //    _context.Designation.Update(designation);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(int id)
        //{
        //    var entity = await _context.Designation.FindAsync(id);
        //    if (entity != null)
        //    {
        //        _context.Designation.Remove(entity);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
