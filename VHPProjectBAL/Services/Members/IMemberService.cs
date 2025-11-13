using Microsoft.AspNetCore.Http;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.DataModel;
using VHPProjectDTOModel.MemberDTO.request;
using VHPProjectDTOModel.MemberDTO.responses;

namespace VHPProjectBAL.Services.Members
{
    public interface IMemberService
    {

     

        Task<MemberUploadResponseDTO> ProcessMemberExcelAsync(IFormFile file, string pathName);
        Task<byte[]> GenerateMemberExcelAsync(int? villageMasterId, int? talukaMasterId);

        Task<Member> GetMemberByIdAsync(int memberId);

        Task<ResultWithDataDTO<MemberResponseRefreshToken>> GetRefreshToken(MemberRequestRefreshToken_DTO refreshToken_DTO, HttpContext httpContext);

        Task<(VerifyLoginResponseDTO, string message)> VerifyLoginAsync(VerifyLoginRequestDTO request);
        
        Task<(MobileExistResponseDTO, string message)> MobileExistAsync(MobileExistRequestDTO request);

        public MemberResponseDTO AddMember(AddMemberRequestDTO request);

        public MemberResponseDTO UpdateMember(UpdateMemberRequestDTO request);

        public MemberResponseDTO GetMember(int memberId);

        public MemberResponseDTO DeleteMember(int memberId);

        Task<ResultWithDataDTO<int>> VerifyLogout(MemberRequestRefreshToken_DTO refreshToken_DTO);

    }
}
