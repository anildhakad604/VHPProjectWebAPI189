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

        Task<ResultWithDataDTO<MemberResponseDTO>> GetMemberAsync(int memberId);

        Task<ResultWithDataDTO<MemberResponseRefreshToken>> GetRefreshToken(MemberRequestRefreshToken_DTO refreshToken_DTO, HttpContext httpContext);

        Task<(VerifyLoginResponseDTO, string message)> VerifyLoginAsync(VerifyLoginRequestDTO request);

        Task<(MobileExistResponseDTO, string message)> MobileExistAsync(MobileExistRequestDTO request);

        Task<ResultWithDataDTO<MemberResponseDTO>> AddMember(AddMemberRequestDTO request);

        Task<ResultWithDataDTO<MemberResponseDTO>> UpdateMember(UpdateMemberRequestDTO request);

        Task<ResultWithDataDTO<MemberResponseDTO>> GetMember(int memberId);

        Task<ResultWithDataDTO<MemberResponseDTO>> DeleteMember(int memberId);
        Task<ResultWithDataDTO<int>> VerifyLogout(MemberRequestRefreshToken_DTO refreshToken_DTO);

        Task<ResultWithDataDTO<List<string>>> UploadFilesAsync(IFormFile[] files, string pathName);
        Task<ResultWithDataDTO<int>> DeleteFilesAsync(List<string> filePaths);
        Task<ResultWithDataDTO<byte[]>> DownloadFilesAsZipAsync(List<string> filePaths);

       

    }
}
