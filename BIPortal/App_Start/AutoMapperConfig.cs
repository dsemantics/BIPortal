using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BIPortal.Mapping;
using Microsoft.Extensions.DependencyInjection;
namespace BIPortal.App_Start
{
    public static class AutoMapperConfig
    {
        private static IServiceProvider serviceProvider;
        public static void Configure()
        {
            var collection = new ServiceCollection();
            var mappingConfiguration = new MapperConfiguration(config => config.AddProfile(new MapperProfile()));
            IMapper mapper = mappingConfiguration.CreateMapper();
            collection.AddSingleton(mapper);
            serviceProvider = collection.BuildServiceProvider();
        }

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSingleton(s => Mapper.Instance);
        //}
    }
    
}