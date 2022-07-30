using AutoMapper;
using Infrastructure.Persistence.Repositories;
using WebApi.Application.Dtos;
using WebApi.Application.Interfaces;
using WebApi.Infrastructure.Contexts;

namespace WebApi.Infrastructure.Repositories
{
    public class ProductSqlRepository: 
        GenericSqlRepository<WebApi.Core.Domain.Product,Guid>,
        IProductRepository
    {
        public ProductSqlRepository(IMapper mapper ,ApplicationDbContext applicationDbContext) 
            : base(mapper,applicationDbContext)
        {
        }
    }
}
