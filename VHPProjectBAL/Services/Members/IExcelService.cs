using Microsoft.AspNetCore.Http;
using VHPProjectDAL.DataModel;

namespace VHPProjectBAL.Services.Members
{
    public interface IExcelService
    {
        Task<List<Member>> ReadMembersFromExcelAsync(IFormFile file);
        byte[] GenerateMemberExcelFile(IEnumerable<Member> members);
    }
}
