using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.Repository.TalukaRepo
{
    public interface ITalukaRepository
    {
        Task<IEnumerable<Talukamaster>> GetActiveTalukasAsync(string? talukaName);
    }
}
