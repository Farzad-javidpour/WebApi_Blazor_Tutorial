using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.AutoMapperSetting
{
    public static class AutoMapperConfiguration
    {
        public static void AddCustomAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddAutoMapper(cfg => cfg.ScanAndCreateProfile(assemblies), assemblies);
        }

        public static void ScanAndCreateProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                                              type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
            
        }
    }
}
