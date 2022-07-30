using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Application.Contracts;

namespace WebApi.Application.Dtos
{
    public class ProductReadDto : BaseDto<ProductReadDto, WebApi.Core.Domain.Product>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public uint Price { get; set; }
        public byte[] RowVersion { get; set; }
    }

    public class ProductCreateDto : BaseDto<ProductCreateDto, WebApi.Core.Domain.Product>, IValidatableObject
    {
        public string Title { get; set; }
        public uint Price { get; set; }

        /// <summary>
        /// Validate All Business Rules That Don't Rely On DataBase
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Equals("hasan", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("اسم نباید حسن باشد", new List<string> { nameof(Title) });
            }
        }
    }

    public class ProductUpdateDto : BaseDto<ProductUpdateDto, WebApi.Core.Domain.Product>, IValidatableObject
    {
        public string Title { get; set; }
        public uint Price { get; set; }
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Validate All Business Rules That Don't Rely On DataBase
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Equals("hasan", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("اسم نباید حسن باشد", new List<string> { nameof(Title) });
            }
        }
    }
}
