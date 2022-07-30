
using System.Collections.Generic;
using Core.Application.Interfaces;

namespace Infrastructure.Common.Wrappers
{
    public class DataResult<T> : IDataResult<T>
    {
        public List<T>? Data { get; set; }
        public IPaginationInfo? PaginationInfo { get; set; }
    }
}
