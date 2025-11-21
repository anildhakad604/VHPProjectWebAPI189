using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.Wall;
using VHPProjectCommonUtility.Logger;
using VHPProjectDTOModel.WallDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WallController : ControllerBase
    {
        private readonly IWallService _service;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public WallController(IWallService service, ILoggerManager logger,IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("AddWall")]
        public async Task<IActionResult> AddWall([FromBody] AddWallRequest req)
        {
            _logger.LogInfo("API: AddWall called");

            var res = await _service.AddWallAsync(req);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"AddWall failed: {res.Message}");
                return BadRequest(res);
            }

            _logger.LogInfo("AddWall succeeded");
            return Ok(res);
        }

        [HttpPut("PostLikeDislike")]
        public async Task<IActionResult> PostLikeDislike([FromBody] WallLikeDislikeRequest req)
        {
            _logger.LogInfo("API: PostLikeDislike called");

            var res = await _service.PostLikeDislikeAsync(req);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"PostLikeDislike failed: {res.Message}");
                return BadRequest(res);
            }

            return Ok(res);
        }

        [HttpPost("AddPostComments")]
        public async Task<IActionResult> AddPostComment([FromBody] AddPostCommentRequest req)
        {
            _logger.LogInfo("API: AddPostComment called");

            var res = await _service.AddPostCommentAsync(req);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"AddPostComment failed: {res.Message}");
                return BadRequest(res);
            }

            return Ok(res);
        }

        [HttpPut("DeleteWall")]
        public async Task<IActionResult> DeleteWall([FromBody] DeleteWallRequest req)
        {
            _logger.LogInfo($"API: DeleteWall called for Id={req.IdWall}");

            var res = await _service.DeleteWallAsync(req.IdWall);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"DeleteWall failed for Id: {req.IdWall}. Message: {res.Message}");

                if (res.IsBusinessError)
                    return NotFound(res);

                return BadRequest(res);
            }

            return Ok(res);
        }

        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment([FromQuery] int idWallPostComments)
        {
            _logger.LogInfo($"API: DeleteComment called for commentId: {idWallPostComments}");

            var res = await _service.DeleteCommentAsync(idWallPostComments);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"DeleteComment failed. Message: {res.Message}");

                if (res.IsBusinessError)
                    return NotFound(res);

                return BadRequest(res);
            }

            return Ok(res);
        }

        [HttpGet("GetWallData")]
        public async Task<IActionResult> GetWallData([FromQuery] GetWallDataRequest req)
        {
            _logger.LogInfo($"API: GetWallData called. Page={req.PageNumber}, Size={req.PageSize}");

            var res = await _service.GetWallDataAsync(req);

            if (!res.IsSuccessful)
            {
                _logger.LogError($"GetWallData failed: {res.Message}");
                return BadRequest(res);
            }

            return Ok(res);
        }
    }
}
