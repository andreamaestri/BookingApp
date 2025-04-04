using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
    #region Enums (Core Classification Types)

    /// <summary>
    /// Defines the primary types of holiday accommodation available in Cornwall.
    /// Keep this list focused on broad categories relevant to the region.
    /// </summary>
    public enum AccommodationType
    {
        Cottage,
        Apartment,
        Flat,
        House,
        Bungalow,
        Farmhouse,
        Lodge,      
        Studio,
        Other       // Catch-all, details in Description.
    }

    /// <summary>
    /// Represents the lifecycle status of a booking.
    /// </summary>
    public enum BookingStatus
    {
        Pending,     // Awaiting confirmation/payment.
        Confirmed,   // Secured, guest expected.
        Cancelled,   // Booking cancelled before stay.
        Completed,   // Stay finished successfully.
        NoShow       // Guest failed to arrive.
    }

    /// <summary>
    /// Key amenities/features often seen in Cornwall holiday lets.
    /// Using an Enum collection is concise and queryable.
    /// </summary>
    public enum AmenityType // Renamed from FeatureType for clarity
    {
        Wifi,
        Parking,          // Consider variants if needed: OffRoadParking, GarageParking
        PetFriendly,
        Kitchen,          // Could be broken down (e.g., BasicKitchen, FullKitchen) if necessary.
        WashingMachine,
        Dishwasher,
        SwimmingPool,     // Specify Private or Shared if needed.
        HotTub,
        Garden,           // Specify Private or Shared if needed.
        WoodBurner,   
        EVCharging,      
        AirConditioning,  
        Heating,          // Assumed standard, but explicit mention can be useful.
        SurfboardStorage, // Relevant for coastal properties.
        BBQ,
        Accessible        // Consider dedicated accessibility features/ratings if needed.
    }

    #endregion

    #region Core Models (Concise & Maintainable)

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

    /// <summary>
    /// Represents a booking made by a guest for a specific period.
    /// </summary>
    public class BookingModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [Required]
        public int GuestId { get; set; } // Link to GuestModel or potentially UserModel

        /// <summary>
        /// Start date of the booking stay (inclusive). Date component is primary. Required.
        /// </summary>
        public required DateTime CheckInDate { get; set; }

        /// <summary>
        /// End date of the booking stay (inclusive). Must be on or after CheckInDate. Required.
        /// </summary>
        public required DateTime CheckOutDate { get; set; } // Validation CheckOutDate >= CheckInDate needed in logic

        /// <summary>
        /// Number of guests for this booking. Must be <= Accommodation.MaxOccupancy. Required.
        /// </summary>
        [Range(1, 100)]
        public required int NumberOfGuests { get; set; }

        /// <summary>
        /// Final calculated price for the entire stay (£ GBP). Required.
        /// </summary>
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal TotalPrice { get; set; }

        /// <summary>
        /// Tracks payment status. Logic for payment processing is external.
        /// </summary>
        public bool IsPaid { get; set; } = false;

        /// <summary>
        /// Current status using the BookingStatus enum. Required.
        /// </summary>
        public required BookingStatus Status { get; set; } = BookingStatus.Pending;

        /// <summary>
        /// Optional notes or requests from the guest.
        /// </summary>
        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        /// <summary>
        /// When the booking was initially created.
        /// </summary>
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        [ForeignKey("AccommodationId")]
        public virtual AccommodationModel? Accommodation { get; set; } // Nullable until loaded

        [ForeignKey("GuestId")]
        public virtual GuestModel? Guest { get; set; } // Nullable until loaded

        // Optional link to review associated with this specific booking
        public virtual ReviewModel? Review { get; set; } // Nullable, not all bookings get reviewed
    }

    /// <summary>
    /// Represents the guest making the booking. Could be merged with a general UserModel
    /// if authentication/accounts are implemented.
    /// </summary>
    public class GuestModel // Consider renaming to UserModel if using accounts
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public required string FirstName { get; set; }

        [StringLength(50)]
        public required string LastName { get; set; }

        /// <summary>
        /// Primary contact email. Should be unique if used for login. Required.
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        /// <summary>
        /// Contact phone number. Required for booking communication.
        /// </summary>
        [Phone]
        [StringLength(30)] // Allow space for international codes
        public required string Phone { get; set; }

        /// <summary>
        /// Optional: Guest's home address.
        /// </summary>
        [StringLength(250)]
        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        public virtual ICollection<BookingModel> Bookings { get; set; } = new List<BookingModel>();
        public virtual ICollection<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }

    /// <summary>
    /// Defines specific date ranges with potential overrides for pricing,
    /// availability, or minimum stay rules for an Accommodation.
    /// </summary>
    public class AvailabilityPeriod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        /// <summary>
        /// Start date of this period (inclusive). Required.
        /// </summary>
        public required DateTime StartDate { get; set; }

        /// <summary>
        /// End date of this period (inclusive). Required. Must be >= StartDate.
        /// </summary>
        public required DateTime EndDate { get; set; } // Validation EndDate >= StartDate needed

        /// <summary>
        /// Overrides Accommodation.BasePricePerNight for this period (£ GBP). Null means use base price.
        /// </summary>
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PricePerNightOverride { get; set; }

        /// <summary>
        /// Explicitly block/unblock dates. False = Blocked (owner stay, maintenance), True = Available (subject to price/bookings).
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Optional: Minimum nights required for bookings starting in this period.
        /// </summary>
        [Range(1, 365)]
        public int? MinimumStayNights { get; set; }

        /// <summary>
        /// Optional internal note (e.g., "Peak Season", "Maintenance Block").
        /// </summary>
        [StringLength(100)]
        public string? Notes { get; set; }

        // --- Navigation Property ---
        [ForeignKey("AccommodationId")]
        public virtual AccommodationModel? Accommodation { get; set; } // Nullable until loaded
    }

    /// <summary>
    /// Represents a guest's review following a stay (linked via Booking).
    /// </summary>
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [Required]
        public int GuestId { get; set; }

        /// <summary>
        /// Links review directly to the completed stay/booking. Required.
        /// </summary>
        [Required]
        public int BookingId { get; set; }

        /// <summary>
        /// Rating score (e.g., 1-5 stars). Required.
        /// </summary>
        [Range(1, 5)]
        public required int Rating { get; set; }

        /// <summary>
        /// Optional textual feedback from the guest.
        /// </summary>
        [StringLength(2000)]
        public string? Comment { get; set; }

        /// <summary>
        /// Optional flag for moderation workflow.
        /// </summary>
        public bool IsApproved { get; set; } = true; // Default depends on business rule

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        [ForeignKey("AccommodationId")]
        public virtual AccommodationModel? Accommodation { get; set; } // Nullable until loaded

        [ForeignKey("GuestId")]
        public virtual GuestModel? Guest { get; set; } // Nullable until loaded

        [ForeignKey("BookingId")]
        public virtual BookingModel? Booking { get; set; } // Nullable until loaded
    }

    #endregion
}