using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Server.Model
{
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
}