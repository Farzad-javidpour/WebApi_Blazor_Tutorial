using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IPaginationInfo
    {
        int CurrentPage { get; set; }
        int PageSize { get; set; }
        int TotalPages { get; set; }
        int TotalCount { get; set; }
        bool HasPreviousPage => CurrentPage > 1;
        bool HasNextPage => CurrentPage < TotalPages;
    }
}
