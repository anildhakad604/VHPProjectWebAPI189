using System;
using System.Collections.Generic;
using System.Text;

namespace VHPProjectCommonUtility.Response
{
    public class ResultWithPageDataDTO<T>
    {
        public string? status { get; set; }
        public int? code { get; set; } = 200;
        public string? message { get; set; } = null;
        public PaginatedData<T>? data { get; set; }
        public Dictionary<string, List<string>>? errors { get; set; } = null;
    }

    public class PaginatedData<T> 
    {
        public int TotalRecords { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public int PageNumber { get; set; } = 0;
        public T? Datas { get; set; }
    }
   
}
