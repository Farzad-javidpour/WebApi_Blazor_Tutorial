using Core.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Core.Domain
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
        public int Price { get; set; }
    }
}
