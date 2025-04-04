using BookingApp.Server.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Dtos
{
    // DTO for displaying a summary in lists
    public class AccommodationSummaryDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public AccommodationType Type { get; set; }
        public required string Town { get; set; }
        public int Bedrooms { get; set; }
        public int MaxOccupancy { get; set; }
        public decimal BasePricePerNight { get; set; }
        public decimal? AverageRating { get; set; }
        public bool HasSeaView { get; set; }
        public string? PrimaryImageUrl { get; set; } 
        public bool IsPetFriendly { get; set; }
    }

    // DTO for displaying detailed information
    public class AccommodationDetailDto : AccommodationSummaryDto // Inherits common fields
    {
        public required string Description { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string PostCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? DistanceToNearestBeachKm { get; set; }
        public int Bathrooms { get; set; }
        public decimal? CleaningFee { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public required int OwnerId { get; set; } // Or OwnerName if joining
        public ICollection<AmenityType> Amenities { get; set; } = new List<AmenityType>();
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>(); // Use ReviewDto
        public ICollection<AvailabilityPeriodDto> AvailabilityPeriods { get; set; } = new List<AvailabilityPeriodDto>(); // Use AvailabilityPeriodDto
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

    // DTO for creating a new accommodation
    public class CreateAccommodationDto
    {
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public required string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public required string Description { get; set; }

        [Required]
        public required AccommodationType Type { get; set; }

        [Required]
        [StringLength(150)]
        public required string AddressLine1 { get; set; }

        [StringLength(100)]
        public string? AddressLine2 { get; set; }

        [Required]
        [StringLength(100)]
        public required string Town { get; set; }

        [Required]
        [RegularExpression(@"^([A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2})$", ErrorMessage = "Invalid UK Postcode format")]
        [StringLength(10)]
        public required string PostCode { get; set; }

        [Range(-90.0, 90.0)]
        public required double Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public required double Longitude { get; set; }

        [Range(0, 1000)]
        public double? DistanceToNearestBeachKm { get; set; }

        [Range(0, 50)]
        public required int Bedrooms { get; set; }

        [Range(1, 50)]
        public required int Bathrooms { get; set; }

        [Required]
        [Range(1, 100)]
        public required int MaxOccupancy { get; set; }

        public bool HasSeaView { get; set; } = false;

        public ICollection<AmenityType> Amenities { get; set; } = new List<AmenityType>();
        public ICollection<string> ImageUrls { get; set; } = new List<string>();

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal BasePricePerNight { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CleaningFee { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SecurityDeposit { get; set; }

        // OwnerId should likely be derived from the authenticated user context, not the DTO
        // public required int OwnerId { get; set; }
    }

    // DTO for updating an existing accommodation
    public class UpdateAccommodationDto : CreateAccommodationDto // Inherit validation and fields
    {
        // No additional fields needed if inheriting, but could add specific update-only fields
        // For PUT, all fields are typically required (or defaults used),
        // For PATCH, you'd have optional fields. Let's assume PUT.
    }

    // --- Supporting DTOs (Example - define fully as needed) ---
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public required string GuestName { get; set; } // Example: Include guest name instead of ID
    }

    public class AvailabilityPeriodDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? PricePerNightOverride { get; set; }
        public bool IsAvailable { get; set; }
        public int? MinimumStayNights { get; set; }
        public string? Notes { get; set; }
    }

    // Filter DTO (mostly unchanged, maybe add amenity list filter)
    public class AccommodationFilter
    {
        public string? Town { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinOccupancy { get; set; } // Added MinOccupancy
        public AccommodationType? Type { get; set; }
        // public bool? PetFriendly { get; set; } // Replaced by Amenities list
        public List<AmenityType>? RequiredAmenities { get; set; } // Filter by multiple amenities
        public bool? HasSeaView { get; set; }
        public string? SortBy { get; set; } // e.g., "price", "rating", "name"
        public string? SortDirection { get; set; } // "asc" or "desc"
        public int PageNumber { get; set; } = 1; // Default to page 1
        public int PageSize { get; set; } = 10; // Default page size
    }

    // Paged Result DTO (unchanged, but good practice)
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}