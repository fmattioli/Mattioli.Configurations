namespace Coderaw.Settings.Http
{
    public class PaginationRequest(int pageNumber, int pageSize)
    {
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int Skip => PageSize * (PageNumber - 1);
    }
}
