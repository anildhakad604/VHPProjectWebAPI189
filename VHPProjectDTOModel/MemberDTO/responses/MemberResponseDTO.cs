namespace VHPProjectDTOModel.MemberDTO.responses
{
    public class MemberResponseDTO
    {
        public int Data { get; set; }  // 1 = success, 0 = fail
        public string Message { get; set; }

        public int MemberId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string MobileNumber { get; set; } = null!;


    }
}
