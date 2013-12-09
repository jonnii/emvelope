using System.Collections.Generic;

namespace Emvelope.Integration.Models
{
    public class PagedEnumerable<T> : List<T>, IPagedEnumerable
    {
        public PagedEnumerable(IEnumerable<T> results)
        {
            AddRange(results);
        }

        public int Page { get; set; }

        public int ItemsPerPage { get; set; }

        public int TotalPages { get; set; }
    }
}
