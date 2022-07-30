using WebApi.Application.Interfaces;

namespace WebApi.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        Task<int> CommitAsync();
    }
}
