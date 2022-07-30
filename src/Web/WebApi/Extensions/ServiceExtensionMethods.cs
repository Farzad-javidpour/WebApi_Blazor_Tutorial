using AutoMapper;
using Core.Application.AutoMapperSetting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WebApi.Extensions
{
    public static class ServiceExtensionMethods
    {
        //-----------------------------------------------------------------------------------------------------------------
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
        //-----------------------------------------------------------------------------------------------------------------

        public static void AddCustomSwagger(this IServiceCollection services, Dictionary<string, string> keyValues)
        {
            services.AddSwaggerGen(c =>
            {
                //c.IncludeXmlComments(string.Format(@"{0}\Shaparak.WebApi.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                foreach (var item in keyValues)
                {
                    c.SwaggerDoc(item.Key, new OpenApiInfo
                    {
                        Version = item.Key,
                        Title = item.Value,
                        Description = "",
                        Contact = new OpenApiContact
                        {
                            Name = "Farzad Javidpour",
                            Email = "javidpour.info@gmail.com",
                        }
                    });
                }
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }
        //-----------------------------------------------------------------------------------------------------------------
    }
}
