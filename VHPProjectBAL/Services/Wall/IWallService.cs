using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.WallDTO.request;
using VHPProjectDTOModel.WallDTO.response;

namespace VHPProjectBAL.Services.Wall
{
    public interface IWallService
    {
        Task<ResultWithDataDTO> AddWallAsync(AddWallRequest request);
        Task<ResultWithDataDTO> PostLikeDislikeAsync(WallLikeDislikeRequest request);
        Task<ResultWithDataDTO> AddPostCommentAsync(AddPostCommentRequest request);
        Task<ResultWithDataDTO> DeleteWallAsync(int idWall);
        Task<ResultWithDataDTO> DeleteCommentAsync(int idComment);
        Task<ResultWithDataDTO<List<WallDataResponse>>> GetWallDataAsync(GetWallDataRequest request);
        
    }
}
