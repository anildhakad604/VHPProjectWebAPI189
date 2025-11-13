namespace VHPProjectDTOModel.MemberDTO.responses
{
    public class MemberUploadResponseDTO
    {
        public int TotalValidRecords { get; set; }
        public List<string> ValidationMessages { get; set; } = new();
    }
}
