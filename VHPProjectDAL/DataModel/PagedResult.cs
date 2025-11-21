using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDAL.DataModel
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        public int TotalPages { get; set; }
    }
}
