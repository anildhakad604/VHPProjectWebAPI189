using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.Designation;
using VHPProjectCommonUtility.Logger;
using VHPProjectDTOModel.DesignationDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;
        private readonly ILoggerManager _logger;

        public DesignationController(IDesignationService designationService,ILoggerManager logger)
        {
            _designationService = designationService;
            _logger = logger;
        }


       
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            _logger.LogInfo("API Called: GET /api/designation/active");

            var result = await _designationService.GetActiveAsync();

            if (!result.IsSuccessful)
            {
                _logger.LogWarn($"Failed to fetch active designations. Msg: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInfo("Successfully fetched active designations.");
            return Ok(result);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInfo($"API Called: GET /api/designation/{id}");

            var data = await _designationService.GetByIdAsync(id);

            if (data == null)
            {
                _logger.LogWarn($"Designation not found. ID: {id}");
                return NotFound(new { Message = "Designation not found." });
            }

            _logger.LogInfo($"Fetched designation successfully. ID: {id}");
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDesignationRequest_DTO request)
        {
            _logger.LogInfo("API Called: POST /api/designation");

            if (string.IsNullOrWhiteSpace(request.DesignationName))
            {
                _logger.LogWarn("Add Designation failed: DesignationName is empty.");
                return BadRequest(new { Message = "Designation Name is required." });
            }

            await _designationService.AddDesignationAsync(request);

            _logger.LogInfo("Designation added successfully.");
            return Ok(new { Message = "Designation added successfully." });
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDesignationRequest_DTO request)
        {
            _logger.LogInfo($"API Called: PUT /api/designation | ID: {request.DesignationId}");

            try
            {
                await _designationService.UpdateDesignationAsync(request);
                _logger.LogInfo($"Designation updated successfully. ID: {request.DesignationId}");

                return Ok(new { Message = "Designation updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update failed: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInfo($"API Called: DELETE /api/designation/{id}");

            try
            {
                await _designationService.DeleteDesignationAsync(id);
                _logger.LogInfo($"Designation deleted successfully. ID: {id}");

                return Ok(new { Message = "Designation deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete failed: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
