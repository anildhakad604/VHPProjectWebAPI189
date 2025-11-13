using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using VHPProjectDAL.DataModel;

namespace VHPProjectBAL.Services.Members
{
    public class ExcelService:IExcelService
    {
        public byte[] GenerateMemberExcelFile(IEnumerable<Member> members)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet("Members");

            ws.Cell(1, 1).Value = "FirstName";
            ws.Cell(1, 2).Value = "LastName";
            ws.Cell(1, 3).Value = "MobileNumber";
            ws.Cell(1, 4).Value = "Village";
            ws.Cell(1, 5).Value = "Taluka";

            int row = 2;
            foreach (var m in members)
            {
                ws.Cell(row, 1).Value = m.FirstName;
                ws.Cell(row, 2).Value = m.LastName;
                ws.Cell(row, 3).Value = m.MobileNumber;
                ws.Cell(row, 4).Value = m.VillageMasterId;
                ws.Cell(row, 5).Value = m.TalukaMasterId;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<List<Member>> ReadMembersFromExcelAsync(IFormFile file)
        {
            var members = new List<Member>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                members.Add(new Member
                {
                    FirstName = row.Cell(1).GetString(),
                    LastName = row.Cell(2).GetString(),
                    MobileNumber = row.Cell(3).GetString(),
                    VillageMasterId = row.Cell(4).GetValue<int?>(),
                    TalukaMasterId = row.Cell(5).GetValue<int?>(),
                    CreatedAt = DateTime.Now,
                    //IsActive = true
                });
            }

            return members;
        }

    }
}
