using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.Members;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.MemberDTO.request;
using VHPProjectDTOModel.MemberDTO.responses;
using VHPProjectWebAPI.Helper.Authorization;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ILoggerManager _loggerManager;
        public MemberController(IMemberService memberService, ILoggerManager loggerManager)
        {
            _memberService = memberService;
            _loggerManager= loggerManager;

        }


        [Authorize(AuthenticationSchemes = "CustomTokenScheme")]
        [HttpPost("MobileExist")]
        public async Task<IActionResult> MobileExist([FromBody] MobileExistRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.MobileNumber))
                return BadRequest("Mobile number is required.");

            var (response, message) = await _memberService.MobileExistAsync(request);
            return Ok(response);
        }

        [HttpPost("VerifyLogin")]
        public async Task<IActionResult> VerifyLogin([FromBody] VerifyLoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.MobileNumber) || string.IsNullOrEmpty(request.OTP))
                return BadRequest("Mobile number and OTP are required.");


            var (data, message) = await _memberService.VerifyLoginAsync(request);
            return Ok(new { data, message });
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> GetRefreshToken([FromBody] MemberRequestRefreshToken_DTO refreshToken_DTO)
        {
            _loggerManager.LogInfo("Entry MemberController => GetRefreshToken");
            ResultWithDataDTO<MemberResponseRefreshToken> resultWithDataDTO =
                new ResultWithDataDTO<MemberResponseRefreshToken> { IsSuccessful = false };

            if (refreshToken_DTO == null)
            {
                resultWithDataDTO.IsBusinessError = true;
                resultWithDataDTO.BusinessErrorMessage = "Error, Data Information posted to the Server is empty. Kindly retry, or contact System Admin.";
                return BadRequest(resultWithDataDTO);
            }
            resultWithDataDTO = await _memberService.GetRefreshToken(refreshToken_DTO, HttpContext);
            _loggerManager.LogInfo("Exit OTPController => GetRefreshToken");
            if (resultWithDataDTO.IsSuccessful)
            {
                return Ok(resultWithDataDTO);
            }
            else
            {
                return BadRequest(resultWithDataDTO);
            }
        }

        [HttpPost("VerifyLogout")]
        public async Task<IActionResult> VerifyLogout([FromBody] MemberRequestRefreshToken_DTO refreshToken_DTO)
        {
            _loggerManager.LogInfo("Entry MemberController => VerifyLogout");
            ResultWithDataDTO<int> resultWithDataDTO =
                new ResultWithDataDTO<int> { IsSuccessful = false };

            if (refreshToken_DTO == null)
            {
                resultWithDataDTO.IsBusinessError = true;
                resultWithDataDTO.BusinessErrorMessage = "Error, Data Information posted to the Server is empty. Kindly retry, or contact System Admin.";
                return BadRequest(resultWithDataDTO);
            }

            resultWithDataDTO = await _memberService.VerifyLogout(refreshToken_DTO);
            _loggerManager.LogInfo("Exit OTPController => VerifyLogout");
            if (resultWithDataDTO.IsSuccessful)
            {
                return Ok(resultWithDataDTO);
            }
            else
            {
                return BadRequest(resultWithDataDTO);
            }

        }




        [Authorize]
        [HttpGet("GetMemberDetails")]
        public async Task<IActionResult> GetMemberDetailsAsync()
        {

            var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MemberId")?.Value;

            if (string.IsNullOrEmpty(memberIdClaim))
                return Unauthorized("Invalid token: MemberId not found.");

            int memberId = int.Parse(memberIdClaim);


            var member = await _memberService.GetMemberByIdAsync(memberId);

            if (member == null)
                return NotFound("Member not found.");

            return Ok(member);


        }

        [HttpPost("AddMember")]
        [Authorize(Policy = Policies.AddMember)]

        public IActionResult AddMember([FromBody] AddMemberRequestDTO request)
        {
            var result = _memberService.AddMember(request);
            return Ok(result);
        }

        [HttpGet("{memberId}")]
        public IActionResult GetMember([FromRoute] int memberId)
        {
            var result = _memberService.GetMember(memberId);
            return Ok(result);
        }

        [HttpPut("UpdateMember")]
        [Authorize(Policy = Policies.UpdateMember)]

        public IActionResult UpdateMember([FromBody] UpdateMemberRequestDTO request)
        {
            var result = _memberService.UpdateMember(request);
            return Ok(result);
        }

        [HttpDelete("DeleteMember/{id}")]
        [Authorize(Policy = Policies.DeleteMember)]

        public IActionResult DeleteMember([FromBody] GetOrDeleteMemberRequestDTO request)
        {
            var result = _memberService.DeleteMember(request.MemberId);
            return Ok(result);
        }


        [HttpPost("UploadMemberExcel")]
        //[Authorize(Policy = "Add Member")]
        public async Task<IActionResult> UploadMemberExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");



            var pathName = Path.GetTempPath();
            var response = await _memberService.ProcessMemberExcelAsync(file, pathName);
            return Ok(response);
        }

        [HttpGet("ExportMemberExcel")]
        public async Task<IActionResult> ExportMemberExcel(int? villageMasterId, int? talukaMasterId)
        {
            var fileBytes = await _memberService.GenerateMemberExcelAsync(villageMasterId, talukaMasterId);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Members.xlsx");
        }



        //[HttpPost("AddMember")]
        //[Authorize(Policy = Policies.AddMember)]
        //public IActionResult AddMember()
        //{
        //    return Ok("Member added successfully.");
        //}

        //[HttpDelete("DeleteMember/{id}")]
        //[Authorize(Policy = Policies.DeleteMember)]
        //public IActionResult DeleteMember(int id)
        //{
        //    return Ok("Member deleted successfully.");
        //}

        //[HttpPost("AddSatsang")]
        //[Authorize(Policy = Policies.AddSatsang)]
        //public IActionResult AddSatsang()
        //{
        //    return Ok("Satsang added successfully.");
        //}

        //[HttpPut("UpdateMember")]
        //[Authorize(Policy = Policies.UpdateMember)]
        //public IActionResult UpdateMember()
        //{
        //    return Ok("Member updated successfully.");
        //}

        //// 📤 Upload Member Excel
        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadMemberExcel([FromForm] IFormFile file, [FromForm] string pathName)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    try
        //    {
        //        MemberUploadResponseDTO result = await _memberService.ProcessMemberExcelAsync(file, pathName);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "Error while processing Excel file.",
        //            error = ex.Message,
        //            stack = ex.StackTrace
        //        });
        //    }
        //}


        //[HttpGet("{memberId:int}")]
        //public async Task<IActionResult> GetMemberById(int memberId)
        //{
        //    var member = await _memberService.GetMemberByIdAsync(memberId);
        //    if (member == null)
        //        return NotFound("Member not found");

        //    return Ok(member);
        //}


        //}
    }
}
