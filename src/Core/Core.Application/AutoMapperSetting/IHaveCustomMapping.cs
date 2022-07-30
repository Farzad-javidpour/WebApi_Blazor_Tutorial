using AutoMapper;

namespace Core.Application.AutoMapperSetting
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
