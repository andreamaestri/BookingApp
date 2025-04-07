using AutoMapper;
using BookingApp.Server.Dtos;
using BookingApp.Server.Model;
using System;
using System.Collections.Generic;
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
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.MapFrom(src => src.AvailabilityPeriods))
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));

            // Review mappings
            CreateMap<ReviewModel, ReviewDto>()
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => 
                    src.Guest != null ? $"{src.Guest.FirstName} {src.Guest.LastName}" : "Anonymous"));

            // Availability Period mappings
            CreateMap<AvailabilityPeriod, AvailabilityPeriodDto>();
            CreateMap<AvailabilityPeriodDto, AvailabilityPeriod>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Accommodation, opt => opt.Ignore());

            // CreateAccommodationDto to AccommodationModel
            CreateMap<CreateAccommodationDto, AccommodationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => 
                    new HashSet<AmenityType>(src.Amenities)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    new List<string>(src.ImageUrls)));

            // UpdateAccommodationDto to AccommodationModel (similar to Create)
            CreateMap<UpdateAccommodationDto, AccommodationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())
                .ForMember(dest => dest.AvailabilityPeriods, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => 
                    new HashSet<AmenityType>(src.Amenities)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    new List<string>(src.ImageUrls)));

            // AccommodationModel to UpdateAccommodationDto (for PATCH operations)
            CreateMap<AccommodationModel, UpdateAccommodationDto>()
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => 
                    new List<AmenityType>(src.Amenities)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    new List<string>(src.ImageUrls)));

            // BookingModel mappings (needed for relationships)
            CreateMap<BookingModel, BookingModel>();
            
            // Booking Summary DTO mapping
            CreateMap<BookingModel, BookingSummaryDto>()
                .ForMember(dest => dest.AccommodationName, opt => opt.MapFrom(src => 
                    src.Accommodation != null ? src.Accommodation.Name : string.Empty))
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => 
                    src.Guest != null ? $"{src.Guest.FirstName} {src.Guest.LastName}" : string.Empty));

            // Booking Detail DTO mapping
            CreateMap<BookingModel, BookingDetailDto>()
                .ForMember(dest => dest.NightsCount, opt => opt.Ignore()) // Calculated property
                .ForMember(dest => dest.Accommodation, opt => opt.MapFrom(src => src.Accommodation))
                .ForMember(dest => dest.Guest, opt => opt.MapFrom(src => src.Guest));

            // Create Booking DTO to BookingModel
            CreateMap<CreateBookingDto, BookingModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.GuestId, opt => opt.Ignore())
                .ForMember(dest => dest.Guest, opt => opt.Ignore())
                .ForMember(dest => dest.Accommodation, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BookingStatus.Confirmed))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()) // Calculated in repository
                .ForMember(dest => dest.CancellationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CancellationReason, opt => opt.Ignore());

            // Guest Summary DTO mapping (needed for booking relationship)
            CreateMap<GuestModel, GuestSummaryDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => 
                    $"{src.FirstName} {src.LastName}"));
        }
    }
}