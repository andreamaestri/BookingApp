using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
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
}