namespace Coderaw.Settings.Common
{
    public class PaginationRequest(int pageNumber, int pageSize) : DateRequest
    {
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int Skip => PageSize * (PageNumber - 1);
    }
}
