using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectBAL.Services.Members
{
    public interface IExcelService
    {
        Task<List<Member>> ReadMembersFromExcelAsync(IFormFile file);
        byte[] GenerateMemberExcelFile(IEnumerable<Member> members);
    }
}
