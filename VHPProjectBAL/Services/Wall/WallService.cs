using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.DataModel;
using VHPProjectDAL.WallRepo;
using VHPProjectDTOModel.WallDTO.request;
using VHPProjectDTOModel.WallDTO.response;

namespace VHPProjectBAL.Services.Wall
{
    public class WallService : IWallService
    {
        private readonly IWallRepository _repo;
        private readonly ILoggerManager _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public WallService(IWallRepository repo, ILoggerManager logger, IWebHostEnvironment env, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _env = env;
            _mapper = mapper;
        }

        
        public async Task<ResultWithDataDTO> AddPostCommentAsync(AddPostCommentRequest request)
        {
            var result = new ResultWithDataDTO();

            try
            {
                _logger.LogInfo("WallService => AddPostCommentAsync invoked.");

                var entity = _mapper.Map<WallPostComments>(request);
                entity.InsertDateTime = DateTime.UtcNow;
                entity.CreatedDate = DateTime.UtcNow;
                entity.Active = 1;

                await _repo.AddCommentAsync(entity);

                result.IsSuccessful = true;
                result.Message = "Comment added successfully.";

                _logger.LogInfo("WallService => AddPostCommentAsync executed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in AddPostCommentAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error adding comment.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


        
        public async Task<ResultWithDataDTO> AddWallAsync(AddWallRequest request)
        {
            var result = new ResultWithDataDTO();

            try
            {
                _logger.LogInfo("WallService => AddWallAsync invoked.");

                var wall = _mapper.Map<VHPProjectDAL.DataModel.Wall>(request.AddWall);
                wall.CreatedDate = DateTime.UtcNow;
                wall.Active = 1;

                int newWallId = await _repo.AddWallAsync(wall);

                
                if (request.AddPostImages != null && request.AddPostImages.Any())
                {
                    var imageEntities = request.AddPostImages.Select(img =>
                    {
                        var entity = _mapper.Map<WallPostImages>(img);
                        entity.WallId = newWallId;
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.Active = 1;
                        return entity;
                    }).ToList();

                    await _repo.AddWallImagesAsync(imageEntities);
                }

                result.IsSuccessful = true;
                result.Message = "Wall added successfully.";

                _logger.LogInfo("WallService => AddWallAsync completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in AddWallAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error adding wall.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


       
        public async Task<ResultWithDataDTO> DeleteCommentAsync(int idComment)
        {
            var result = new ResultWithDataDTO();

            try
            {
                _logger.LogInfo($"WallService => DeleteCommentAsync({idComment}) invoked.");

                bool deleted = await _repo.SoftDeleteCommentAsync(idComment);

                if (!deleted)
                {
                    result.IsSuccessful = false;
                    result.IsBusinessError = true;
                    result.Message = "Comment not found.";
                    return result;
                }

                result.IsSuccessful = true;
                result.Message = "Comment deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in DeleteCommentAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error deleting comment.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


        public async Task<ResultWithDataDTO> DeleteWallAsync(int idWall)
        {
            var result = new ResultWithDataDTO();

            try
            {
                _logger.LogInfo($"WallService => DeleteWallAsync({idWall}) invoked.");

                bool deleted = await _repo.SoftDeleteWallAsync(idWall);

                if (!deleted)
                {
                    result.IsSuccessful = false;
                    result.IsBusinessError = true;
                    result.Message = "Wall post not found.";
                    return result;
                }

                result.IsSuccessful = true;
                result.Message = "Wall deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in DeleteWallAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error deleting wall.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


       
        public async Task<ResultWithDataDTO<List<WallDataResponse>>> GetWallDataAsync(GetWallDataRequest request)
        {
            var result = new ResultWithDataDTO<List<WallDataResponse>>();

            try
            {
                _logger.LogInfo("WallService => GetWallDataAsync invoked.");

                int skip = (request.PageNumber - 1) * request.PageSize;

                var data = await _repo.GetWallDataAsync(request, skip, request.PageSize);
                var total = await _repo.GetTotalWallCountAsync();

                result.Data = data;
                result.TotalCount = total;
                result.CurrentPage = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalPages = (int)Math.Ceiling(total / (double)request.PageSize);
                result.previousPage = request.PageNumber > 1 ? "true" : "false";
                result.nextPage = request.PageNumber < result.TotalPages ? "true" : "false";

                result.IsSuccessful = true;
                result.Message = "Wall data fetched successfully.";

                _logger.LogInfo("WallService => GetWallDataAsync completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in GetWallDataAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error fetching wall data.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


        
        public async Task<ResultWithDataDTO> PostLikeDislikeAsync(WallLikeDislikeRequest request)
        {
            var result = new ResultWithDataDTO();

            try
            {
                _logger.LogInfo("WallService => PostLikeDislikeAsync invoked.");

                var entity = _mapper.Map<WallPostLike>(request);
                entity.CreatedDate = DateTime.UtcNow;
                entity.Active = 1;

                await _repo.AddOrUpdateLikeAsync(entity);

                result.IsSuccessful = true;
                result.Message = "Like/Dislike updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in PostLikeDislikeAsync", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error updating like/dislike status.";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
