namespace Coderaw.Settings.Common
{
    public record PageFilterRequest
    {
        private const int LowerBoundPageNumber = 1;

        private int page;

        private int pageSize = 60;

        public int Page
        {
            get => page;
            set => page = value < LowerBoundPageNumber ? LowerBoundPageNumber : value;
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value < 1 ? 1 : value;
        }
    }
}
