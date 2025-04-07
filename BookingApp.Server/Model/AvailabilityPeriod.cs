using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
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
}