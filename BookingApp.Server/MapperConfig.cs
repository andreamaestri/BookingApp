using AutoMapper;
using BookingApp.Server.Mapping;
using System;

namespace BookingApp.Server
{
    public static class MapperConfig
    {
        public static IMapper InitializeAutomapper()
        {
            // Create the mapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                // Register the MappingProfile that contains all our mappings
                cfg.AddProfile<MappingProfile>();
                
                // You can add any additional mapping profiles here if needed
                // cfg.AddProfile<OtherMappingProfile>();
            });

            // Validate the configuration (optional but recommended)
            config.AssertConfigurationIsValid();
            
            // Create an instance of Mapper and return it
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}