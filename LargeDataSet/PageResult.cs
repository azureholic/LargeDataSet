using System.Data;

namespace LargeDataSet
{
    public class PageResult
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public string NextPage { get; set; }
        public DataTable PageData { get; set; }
        
    }
}
