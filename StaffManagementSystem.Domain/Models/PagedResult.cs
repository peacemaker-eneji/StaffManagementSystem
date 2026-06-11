using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManagementSystem.Domain.Models {
    public class PagedResult<T> {
        public IEnumerable<T> Items { get; }
        public int TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }

        public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize) {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
            TotalPages = (totalCount+pageSize-1) / pageSize; // ceil division
            HasNextPage = Page < TotalPages;
            HasPreviousPage = Page > 1;
        }
    }
}
