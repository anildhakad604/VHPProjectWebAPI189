using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.MemberRepo
{
    public interface IMemberRepository
    {

        Task AddMembers(List<Member> members);

        Task<bool> MobileExistsAsync(string mobileNumber);

        Task<List<Member>> GetMembersByFilter(int? villageMasterId, int? talukaMasterId);

        Task<Member?> GetMemberByMobileAsync(string mobileNumber);

        Task<Member?> GetMemberByIdAsync(int memberId);

        Task<Member?> GetPlainMemberById(int memberId);

        Task<int> AddRefreshTokenAsync(Refreshtoken refreshTokentoken);
        Task<Refreshtoken> GetRefreshTokenDetails(string refreshToken);
        Task<int> UpdateRefreshTokenAsync(Refreshtoken refreshToken);


        void AddMember(Member member);
        void UpdateMember(Member member);
       
        void DeleteMember(Member member);

        bool IsMobileUnique(string mobileNumber, int? memberId = null);

      


    }
}
