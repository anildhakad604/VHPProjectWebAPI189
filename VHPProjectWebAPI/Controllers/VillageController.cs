using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.VillageMaster;
using VHPProjectDTOModel.VillageDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillageController : ControllerBase
    {
        private readonly IVillageService _villageService;

        public VillageController(IVillageService villageService)
        {
            _villageService = villageService;
        }

        [HttpGet("GetActiveVillages")]
        public async Task<IActionResult> GetActiveVillages([FromQuery] VillageListRequest_DTO request)
        {
            var result = await _villageService.GetActiveVillagesAsync(request);

            if (result.IsSuccessful)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
