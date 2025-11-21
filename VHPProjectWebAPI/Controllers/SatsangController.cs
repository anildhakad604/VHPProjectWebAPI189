using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.SatsangService;
using VHPProjectDTOModel.SatsangDTO.request;
using VHPProjectWebAPI.Helper.Authorization;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatsangController : ControllerBase
    {
        private readonly ISatsangService _satsangService;

        public SatsangController(ISatsangService satsangService)
        {
            _satsangService = satsangService;
        }


        [HttpPost("AddSatsang")]
        [Authorize(Policy = Policies.AddSatsang)]

        public async Task<IActionResult> AddSatsang([FromBody] AddSatsangRequest_DTO request)
        {
            if (request == null)
                return BadRequest("Invalid request data.");

            var response = await _satsangService.AddSatsangAsync(request);
            return Ok(response);
        }

        [HttpGet("GetSatsangDetails")]
        public async Task<IActionResult> GetSatsangDetails(
            int? villageMasterId, int? talukaMasterId, string? templeName,
            DateTime? fromDate, DateTime? toDate, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("PageNumber and PageSize must be greater than zero.");

            var response = await _satsangService.GetSatsangDetailsAsync(
                villageMasterId, talukaMasterId, templeName, fromDate, toDate, pageNumber, pageSize);

            return Ok(response);
        }
    }
}
