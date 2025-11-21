using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.DesignationRepo
{
    public interface IDesignationRepository
    {
        Task<IEnumerable<Designation>> GetActiveDesignationAsync();
        Task<Designation?> GetByIdAsync(int id);
        Task AddAsync(Designation designation);
        Task UpdateAsync(Designation designation);
        Task DeleteAsync(int id);

    }
}
