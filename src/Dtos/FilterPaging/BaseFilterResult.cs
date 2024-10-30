namespace PetProject
{
    public class BaseFilterResult
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalRecords { get; set; }

        public BaseFilterResult(int currentPage, int pageSize, int totalPage, int totalRecords)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPage = totalPage;
            TotalRecords = totalRecords;
        }
    }
}
