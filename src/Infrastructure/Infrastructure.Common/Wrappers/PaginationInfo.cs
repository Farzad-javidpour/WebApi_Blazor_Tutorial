using System;
using Core.Application.Interfaces;

namespace Infrastructure.Common.Wrappers
{
    public class PaginationInfo : IPaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}