using Core.Application.Interfaces;
using WebApi.Application.Dtos;

namespace WebApi.Application.Interfaces
{
    public interface IProductRepository : IGenericSqlRepository<WebApi.Core.Domain.Product,Guid>
    {
    }
}
