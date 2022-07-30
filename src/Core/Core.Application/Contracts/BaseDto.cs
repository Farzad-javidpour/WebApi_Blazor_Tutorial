using AutoMapper;
using Core.Application.AutoMapperSetting;
using Core.Domain.Contracts;

namespace Core.Application.Contracts
{
    public abstract class BaseDto<TDto, TEntity, TKey> : IHaveCustomMapping
        where TEntity : class, IEntity<TKey>, new()
    {
        public TEntity CreateEntity(IMapper mapper)
        {
            return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        }

        public TEntity UpdateEntity(IMapper mapper, TEntity entity)
        {
            return mapper.Map(CastToDerivedClass(mapper, this), entity);
        }

        public static TDto CreateDto(IMapper mapper, TEntity model)
        {
            return mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(IMapper mapper, BaseDto<TDto, TEntity, TKey> baseInstance)
        {
            return mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            //Ignore any property of source that dose not contains in destination 
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }

    public abstract class BaseDto<TDto, TEntity> : BaseDto<TDto, TEntity, Guid>
        where TDto : class, new()
        where TEntity : class, IEntity<Guid>, new()
    {

    }
}
