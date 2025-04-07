using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
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

        /// <summary>
        /// Date when the booking was cancelled, if applicable.
        /// </summary>
        public DateTime? CancellationDate { get; set; }

        /// <summary>
        /// Reason provided for cancellation, if applicable.
        /// </summary>
        [StringLength(500)]
        public string? CancellationReason { get; set; }

        // --- Navigation Properties ---
        [ForeignKey("AccommodationId")]
        public virtual AccommodationModel? Accommodation { get; set; } // Nullable until loaded

        [ForeignKey("GuestId")]
        public virtual GuestModel? Guest { get; set; } // Nullable until loaded

        // Optional link to review associated with this specific booking
        public virtual ReviewModel? Review { get; set; } // Nullable, not all bookings get reviewed
    }
}