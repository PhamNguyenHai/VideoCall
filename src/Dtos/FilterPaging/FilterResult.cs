using System.Collections.Generic;

namespace PetProject
{
    public class FilterResult<TType> : BaseFilterResult
    {
        public IEnumerable<TType>? Data { get; set; }
        public FilterResult(int currentPage, int pageSize, int totalPage, int totalRecords, IEnumerable<TType> data)
            : base(currentPage, pageSize, totalPage, totalRecords)
        {
            Data = new List<TType>();
            Data = data;
        }
    }
}
