namespace VHPProjectCommonUtility.Response
{
    public class ResultWithDataDTO<T> : ResultWithDataDTO
    {
        public T? Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string previousPage { get; set; }
        public string nextPage { get; set; }

    }
    
}
