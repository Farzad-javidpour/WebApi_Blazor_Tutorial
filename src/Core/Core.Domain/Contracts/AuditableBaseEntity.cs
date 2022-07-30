using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Contracts
{
    public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>
    {
        //=====================================================================================================
        [MaxLength(64)]
        public string CreatedBy { get; set; }
        [MaxLength(15)]
        public string CreatorIpAddress { get; set; }
        public DateTime CreateDateTime { get; set; }
        //=====================================================================================================
        [MaxLength(64)]
        public string LastModifiedBy { get; set; }
        [MaxLength(15)]
        public string LastModifierIpAddress { get; set; }
        public DateTime? LastModifyDateTime { get; set; }
        //=====================================================================================================
    }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<Guid>
    {

    }
}
