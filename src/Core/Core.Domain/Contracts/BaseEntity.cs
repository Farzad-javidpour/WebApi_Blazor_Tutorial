using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Contracts
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        //________________________________________________________________________________
        [Key]
        public virtual TKey Id { get; set; }
        //________________________________________________________________________________
        public bool IsDeleted { get ; set ; }
        //________________________________________________________________________________
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        //________________________________________________________________________________
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {

    }
}
