using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.TalukaRepo
{
    public interface ITalukaRepository
    {
        Task<IEnumerable<Talukamaster>> GetActiveTalukasAsync(string? talukaName);
    }
}
