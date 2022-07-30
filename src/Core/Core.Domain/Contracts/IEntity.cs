using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts
{
    public interface IEntity
    {
        bool IsDeleted { get; set; }
    }
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }
}
