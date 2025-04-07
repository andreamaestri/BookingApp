using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
    /// <summary>
    /// Represents a rentable holiday property in Cornwall. Optimized for clarity and ORM usage.
    /// Assumes GBP (£) for all monetary values unless otherwise specified.
    /// </summary>
    public class AccommodationModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Public-facing title of the property listing (e.g., "Cosy Fisherman's Cottage in St Ives"). Required.
        /// </summary>
        [StringLength(150, MinimumLength = 5)]
        public required string Title { get; set; }

        /// <summary>
        /// Alternative name for the property. Can be used for internal reference.
        /// </summary>
        [StringLength(150)]
        public string? Name { get; set; }

        /// <summary>
        /// Detailed description. Can support simple formatting (e.g., paragraphs). Required.
        /// </summary>
        [StringLength(4000)]
        public required string Description { get; set; }

        /// <summary>
        /// Primary type of the property. Required.
        /// </summary>
        public required AccommodationType Type { get; set; }

        // --- Location (Cornwall Focus) ---
        // Kept flat for simplicity; refactor to a nested Location object if complexity grows.

        /// <summary>
        /// First line of the address (e.g., "Seaview Cottage", "12 Fore Street"). Required.
        /// </summary>
        [StringLength(150)]
        public required string AddressLine1 { get; set; }

        /// <summary>
        /// Optional second address line.
        /// </summary>
        [StringLength(100)]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Key search/filter parameter - the town or village in Cornwall (e.g., "St Ives", "Padstow", "Mousehole"). Required.
        /// </summary>
        [StringLength(100)]
        public required string Town { get; set; }

        /// <summary>
        /// Standard UK Postcode (e.g., "TR26 1HE"). Required for location accuracy.
        /// </summary>
        [RegularExpression(@"^([A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2})$", ErrorMessage = "Invalid UK Postcode format")]
        [StringLength(10)]
        public required string PostCode { get; set; }

        /// <summary>
        /// GPS Latitude. Use decimal for precision if needed, double is often sufficient. Required for mapping.
        /// </summary>
        [Range(-90.0, 90.0)]
        public required double Latitude { get; set; }

        /// <summary>
        /// GPS Longitude. Required for mapping.
        /// </summary>
        [Range(-180.0, 180.0)]
        public required double Longitude { get; set; }

        /// <summary>
        /// Approx distance to the nearest beach in kilometers. Highly relevant in Cornwall. Nullable if not applicable/known.
        /// </summary>
        [Range(0, 1000)] // Sensible range check
        public double? DistanceToNearestBeachKm { get; set; }

        // --- Property Capacity & Core Features ---

        [Range(0, 50)] // 0 for Studios maybe?
        public required int Bedrooms { get; set; }

        [Range(1, 50)] // Assume at least one WC/Bathroom facility.
        public required int Bathrooms { get; set; }

        /// <summary>
        /// Strict maximum number of guests allowed. Critical for compliance and booking validation. Required.
        /// </summary>
        [Range(1, 100)]
        public required int MaxOccupancy { get; set; }

        /// <summary>
        /// Subjective but important feature: Does the property have discernible sea views?
        /// </summary>
        public bool HasSeaView { get; set; } = false;

        /// <summary>
        /// Key features/amenities offered. Use the AmenityType enum. More maintainable than individual booleans.
        /// </summary>
        public ICollection<AmenityType> Amenities { get; set; } = new HashSet<AmenityType>();

        // --- Media ---

        /// <summary>
        /// List of public URLs for property images. Order might be significant (e.g., first is primary).
        /// </summary>
        public ICollection<string> ImageUrls { get; set; } = new List<string>();

        // --- Pricing (Defaults - can be overridden by AvailabilityPeriod) ---

        /// <summary>
        /// Default price per night (£ GBP). Required.
        /// </summary>
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal BasePricePerNight { get; set; }

        /// <summary>
        /// Optional fixed cleaning fee per booking (£ GBP).
        /// </summary>
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CleaningFee { get; set; }

        /// <summary>
        /// Optional refundable security deposit (£ GBP).
        /// </summary>
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SecurityDeposit { get; set; }

        // --- Relationships & Audit ---

        /// <summary>
        /// Foreign Key to the Owner/Host User record. Required.
        /// </summary>
        public required int OwnerId { get; set; }
        // public virtual UserModel? Owner { get; set; } // Navigation property (nullable until loaded)

        /// <summary>
        /// Calculated average guest rating (1-5). Null if no reviews. Updated via application logic/triggers.
        /// </summary>
        [Range(1.0, 5.0)]
        public decimal? AverageRating { get; set; }

        /// <summary>
        /// Calculated total number of reviews. Updated via application logic/triggers.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int ReviewCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties (for ORM - initialized, nullable until loaded) ---
        public virtual ICollection<BookingModel> Bookings { get; set; } = new List<BookingModel>();
        public virtual ICollection<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
        public virtual ICollection<AvailabilityPeriod> AvailabilityPeriods { get; set; } = new List<AvailabilityPeriod>();
    }
}