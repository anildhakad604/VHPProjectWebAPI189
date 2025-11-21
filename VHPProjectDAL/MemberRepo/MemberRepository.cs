using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.MemberRepo
{
    public class MemberRepository : IMemberRepository
    {
        private readonly MasterProjContext _context;

        private readonly ILoggerManager _loggerManager;

        public MemberRepository(MasterProjContext context, ILoggerManager loggerManager)
        {
            _context = context;
            _loggerManager = loggerManager;

        }

        public async Task AddMembers(List<Member> members)
        {
            _loggerManager.LogInfo("Entry MemberRepository => AddMember");
            await _context.Member.AddRangeAsync(members);
            await _context.SaveChangesAsync();
            _loggerManager.LogInfo("Exit MemberRepository => AddMember");

        }

        public async Task<List<Member>> GetMembersByFilter(int? villageMasterId, int? talukaMasterId)
        {
            var query = _context.Member.AsQueryable();
            if (villageMasterId.HasValue)
                query = query.Where(m => m.VillageMasterId == villageMasterId.Value);
            if (talukaMasterId.HasValue)
                query = query.Where(m => m.TalukaMasterId == talukaMasterId.Value);
            return await query.ToListAsync();
        }

        public async Task<bool> MobileExistsAsync(string mobileNumber)
        {
            _loggerManager.LogInfo("Entry MemberRepository => MobileExists");
            var existingMember = await _context.Member.AnyAsync(x => x.MobileNumber == mobileNumber);
            _loggerManager.LogInfo("Exit MemberRepository => MobileExists");

            return existingMember;

        }




        public async Task<int> AddRefreshTokenAsync(Refreshtoken refreshToken)
        {
            _loggerManager.LogInfo("Entry MemberRepository => AddRefreshToken");
            await _context.Refreshtoken.AddAsync(refreshToken);
            var result = await _context.SaveChangesAsync();
            _loggerManager.LogInfo("Exit MemberRepository => AddRefreshToken");

            return result;


        }

        public async Task<Member?> GetMemberByIdAsync(int memberId)
        {
            return await _context.Member
       .FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<Member> GetMemberByMobileAsync(string mobileNumber)
        {
            return await Task.FromResult(
                _context.Member.FirstOrDefault(m => m.MobileNumber == mobileNumber)
            );
        }

        public async Task<Refreshtoken?> GetRefreshTokenDetails(string refreshToken)
        {
            _loggerManager.LogInfo("Entry MemberRepository => GetRefreshToken");

            var tokenDetails = await _context.Refreshtoken.Where(rt => rt.Value == refreshToken).FirstOrDefaultAsync();

            _loggerManager.LogInfo("Exit MemberRepository => GetRefreshToken");

            return tokenDetails;


        }

        public async Task<int> UpdateRefreshTokenAsync(Refreshtoken refreshtoken)
        {
            _loggerManager.LogInfo("Entry MemberRepository => UpdateMember");

            _context.Refreshtoken.Update(refreshtoken);

            _loggerManager.LogInfo("Exit MemberRepository => UpdateMember");
            return refreshtoken.IdRefreshToken;

        }

        public async Task<Member?> GetPlainMemberById(int memberId)
        {
            _loggerManager.LogInfo("Entry MemberRepository => GetPlainMemberById");
            var member = await _context.Member.Where(m => m.MemberId == memberId).FirstOrDefaultAsync();
            _loggerManager.LogInfo("Exit MemberRepository => GetPlainMemberById");

            return member;
        }

        public void AddMember(Member member)
        {
            _context.Member.Add(member);
            _context.SaveChanges();
        }

        public void UpdateMember(Member member)
        {
            _context.Member.Update(member);
            _context.SaveChanges();
        }

        public Member GetMember(int memberId)
        {
            return _context.Member.FirstOrDefault(m => m.MemberId == memberId);
        }

        public void DeleteMember(Member member)
        {
            _context.Member.Remove(member);
            _context.SaveChanges();
        }

        public bool IsMobileUnique(string mobileNumber, int? memberId = null)
        {
            return !_context.Member.Any(m => m.MobileNumber == mobileNumber && m.MemberId != memberId);
        }
    }
}
