using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.Designation;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDesignations()
        {
            var result = await _designationService.GetActiveDesignationsAsync();
            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var result = await _designationService.GetByIdAsync(id);
        //    if (result == null)
        //        return NotFound("Designation not found.");
        //    return Ok(result);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddDesignation([FromBody] AddDesignationRequest_DTO request)
        //{
        //    await _designationService.AddDesignationAsync(request);
        //    return Ok("Designation added successfully.");
        //}

        //[HttpPut]
        //public async Task<IActionResult> UpdateDesignation([FromBody] UpdateDesignationRequest_DTO request)
        //{
        //    await _designationService.UpdateDesignationAsync(request);
        //    return Ok("Designation updated successfully.");
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDesignation(int id)
        //{
        //    await _designationService.DeleteDesignationAsync(id);
        //    return Ok("Designation deleted successfully.");
        //}
    }
}
