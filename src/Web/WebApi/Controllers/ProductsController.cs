using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Application.Contracts;
using Infrastructure.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Dtos;
using WebApi.Core.Domain;
using WebApi.Infrastructure;

namespace WebApi.Controllers
{
    public class ProductsController : BaseApiController
    {
        public IMapper Mapper { get; set; }
        public IUnitOfWork UnitOfWork { get; }

        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route(template: "")]
        public async Task<ApiResponse> Filter([FromQuery] FilterDto dto)
        {
            var result = await UnitOfWork.Products.FilterAsync(dto.PagingSetting, dto.FilterString);
            return await ApiResponse.SuccessAsync(result.Data, result.PaginationInfo);
        }

        [HttpGet]
        [Route(template: "{id}")]
        public async Task<ApiResponse> GetOneAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var entity =  await UnitOfWork.Products.GetByIdAsync(id, cancellationToken);

            return await ApiResponse.SuccessAsync(ProductReadDto.CreateDto(Mapper, entity));
        }

        [HttpPost]
        [Route(template: "")]
        public async Task<ApiResponse> Create([FromBody] ProductCreateDto dto, CancellationToken cancellationToken)
        {
            var entity = dto.CreateEntity(Mapper);

            await UnitOfWork.Products.InsertAsync(entity, cancellationToken);
            await UnitOfWork.CommitAsync();

            return await ApiResponse.SuccessAsync(ProductReadDto.CreateDto(Mapper, entity));
        }

        [HttpPut]
        [Route(template: "{id}")]
        public async Task<ApiResponse> Update([FromRoute]Guid id, [FromBody] ProductUpdateDto dto, CancellationToken cancellationToken)
        {
            var entity = dto.CreateEntity(Mapper);
            entity.Id = id;

            UnitOfWork.Products.Update(entity);
            await UnitOfWork.CommitAsync();

            return await ApiResponse.SuccessAsync(ProductReadDto.CreateDto(Mapper, entity));
        }

        [HttpDelete]
        [Route(template: "{id}")]
        public async Task<ApiResponse> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var entity = await UnitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return await ApiResponse.FailAsync("Not Found");
            }

            await UnitOfWork.Products.DeleteAsync(entity,cancellationToken);
            await UnitOfWork.CommitAsync();

            return await ApiResponse.SuccessAsync(ProductReadDto.CreateDto(Mapper, entity));
        }

        [HttpDelete]
        [Route(template: "SoftDelete/{id}")]
        public async Task<ApiResponse> SoftDelete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var entity = await UnitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return await ApiResponse.FailAsync("Not Found");
            }

            entity.IsDeleted = true;
            UnitOfWork.Products.Update(entity);
            await UnitOfWork.CommitAsync();

            return await ApiResponse.SuccessAsync(ProductReadDto.CreateDto(Mapper, entity));
        }
    }
}
