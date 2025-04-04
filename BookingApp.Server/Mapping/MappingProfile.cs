using AutoMapper;
using BookingApp.Server.Dtos;
using BookingApp.Server.Model;
using System.Linq;

namespace BookingApp.Server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Accommodation Summary DTO mappings
            CreateMap<AccommodationModel, AccommodationSummaryDto>()
                .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src => src.ImageUrls.FirstOrDefault()))
                .ForMember(dest => dest.IsPetFriendly, opt => opt.MapFrom(src => src.Amenities.Contains(AmenityType.PetFriendly)));

            // Accommodation Detail DTO mappings (inherits from Summary)
            CreateMap<AccommodationModel, AccommodationDetailDto>()
                .IncludeBase<AccommodationModel, AccommodationSummaryDto>()
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.MapFrom(src => src.AvailabilityPeriods));

            // Review mappings
            CreateMap<ReviewModel, ReviewDto>()
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => 
                    src.Guest != null ? $"{src.Guest.FirstName} {src.Guest.LastName}" : "Anonymous"));

            // Availability Period mappings
            CreateMap<AvailabilityPeriod, AvailabilityPeriodDto>();
            CreateMap<AvailabilityPeriodDto, AvailabilityPeriod>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // CreateAccommodationDto to AccommodationModel
            CreateMap<CreateAccommodationDto, AccommodationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => 
                    new System.Collections.Generic.HashSet<AmenityType>(src.Amenities)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    new System.Collections.Generic.List<string>(src.ImageUrls)));

            // UpdateAccommodationDto to AccommodationModel (similar to Create)
            CreateMap<UpdateAccommodationDto, AccommodationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => 
                    new System.Collections.Generic.HashSet<AmenityType>(src.Amenities)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    new System.Collections.Generic.List<string>(src.ImageUrls)));

            // AccommodationModel to UpdateAccommodationDto (for PATCH operations)
            CreateMap<AccommodationModel, UpdateAccommodationDto>();
        }
    }
}