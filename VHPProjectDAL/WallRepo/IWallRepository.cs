using VHPProjectDAL.DataModel;
using VHPProjectDTOModel.WallDTO.request;
using VHPProjectDTOModel.WallDTO.response;

namespace VHPProjectDAL.WallRepo
{
    public interface IWallRepository
    {
        Task<int> AddWallAsync(Wall wall);
        Task AddWallImagesAsync(IEnumerable<WallPostImages> images);
        Task AddOrUpdateLikeAsync(WallPostLike like);
        Task<int> AddCommentAsync(WallPostComments comment);
        Task<bool> SoftDeleteWallAsync(int wallId);
        Task<bool> SoftDeleteCommentAsync(int commentId);
        Task<int> GetTotalWallCountAsync();
        Task<List<WallDataResponse>> GetWallDataAsync(GetWallDataRequest req, int skip, int take);
        Task<WallPostLike?> GetLikeByMemberAndWallAsync(int memberId, int wallId);
    }
}
