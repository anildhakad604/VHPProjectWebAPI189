using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.TalukaMaster;
using VHPProjectDTOModel.TalukaDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalukaController : ControllerBase
    {
        private readonly ITalukaService _talukaService;

        public TalukaController(ITalukaService talukaService)
        {
            _talukaService = talukaService;
        }

        [HttpGet("GetActiveTalukas")]
        public async Task<IActionResult> GetActiveTalukas([FromQuery] string? talukaName)
        {
            var request = new TalukaListRequest_DTO { TalukaName = talukaName };
            var response = await _talukaService.GetActiveTalukasAsync(request);
            return Ok(response);
        }
    }
}
