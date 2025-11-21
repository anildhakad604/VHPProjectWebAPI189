namespace VHPProjectCommonUtility.Response
{
    public class ResultWithDataDTO
    {
        public string Message { get; set; }
        public List<string> ValidationMessages { get; set; } = new List<string>();
        public bool IsSuccessful { get; set; }
        public bool IsBusinessError { get; set; }
        public bool IsSystemError { get; set; }
        public string SystemErrorMessage { get; set; }
        public string BusinessErrorMessage { get; set; }
    }
}
