namespace Spoiler.Api.Models
{
    public class BlogSearch
    {
        private const int MAX_PAGE_SIZE = 100;

        public string? Keyword { get; set; }
        
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
            }
        }
    }

    public class BlogSearchResult
    {
        public int CurrentPageNumber { get; private set; }
        public int TotalItemsCount { get; private set; }
        public int TotalNumberOfPages { get; private set; }
        public IReadOnlyCollection<BlogDto>? Items { get; private set; }
        public bool HasPrevious => CurrentPageNumber > 1 && CurrentPageNumber - 1 <= TotalNumberOfPages;
        public bool HasNext => CurrentPageNumber < TotalNumberOfPages;

        public BlogSearchResult(BlogSearch search, IReadOnlyCollection<BlogDto> result, int count)
        {
            CurrentPageNumber = search.PageNumber;
            TotalItemsCount = count;
            TotalNumberOfPages = (int)Math.Ceiling(TotalItemsCount / (double)search.PageSize);
            Items = result;
        }
    }
}
