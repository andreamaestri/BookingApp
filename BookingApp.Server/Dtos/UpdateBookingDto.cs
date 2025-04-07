using BookingApp.Server.Model;

namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// DTO for updating an existing booking
    /// </summary>
    public class UpdateBookingDto
    {
        /// <summary>
        /// Updated check-in date
        /// </summary>
        public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// Updated check-out date
        /// </summary>
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// Updated number of guests
        /// </summary>
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int? NumberOfGuests { get; set; }

        /// <summary>
        /// Updated booking status
        /// </summary>
        public BookingStatus? Status { get; set; }

        /// <summary>
        /// Updated special requests
        /// </summary>
        [MaxLength(500, ErrorMessage = "Special requests cannot exceed 500 characters")]
        public string SpecialRequests { get; set; }
    }
}