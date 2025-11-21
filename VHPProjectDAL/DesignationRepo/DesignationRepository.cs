using Microsoft.EntityFrameworkCore;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.DesignationRepo
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly MasterProjContext _context;



        public DesignationRepository(MasterProjContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<Designation>> GetActiveDesignationAsync()
        {
            return await _context.Designation
                .Where(d => d.IsActive == true)
                .ToListAsync();
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _context.Designation
                .FirstOrDefaultAsync(x => x.DesignationId == id);
        }

        public async Task AddAsync(Designation designation)
        {
            await _context.Designation.AddAsync(designation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Designation designation)
        {
            _context.Designation.Update(designation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.Designation.FindAsync(id);
            if (record != null)
            {
                _context.Designation.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
