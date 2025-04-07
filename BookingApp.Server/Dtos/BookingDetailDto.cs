using BookingApp.Server.Model;

namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// Detailed DTO for a booking with all information
    /// </summary>
    public class BookingDetailDto
    {
        /// <summary>
        /// Booking ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Accommodation ID
        /// </summary>
        public int AccommodationId { get; set; }

        /// <summary>
        /// Accommodation information
        /// </summary>
        public AccommodationSummaryDto Accommodation { get; set; }

        /// <summary>
        /// Guest ID
        /// </summary>
        public int GuestId { get; set; }

        /// <summary>
        /// Guest information
        /// </summary>
        public GuestSummaryDto Guest { get; set; }

        /// <summary>
        /// Check-in date
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// Check-out date
        /// </summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// Number of guests
        /// </summary>
        public int GuestCount { get; set; }

        /// <summary>
        /// Total price for the booking
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Booking status
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// Date when the booking was made
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Special requests from the guest
        /// </summary>
        public string SpecialRequests { get; set; }

        /// <summary>
        /// Cancellation date if the booking was cancelled
        /// </summary>
        public DateTime? CancellationDate { get; set; }

        /// <summary>
        /// Reason for cancellation if the booking was cancelled
        /// </summary>
        public string CancellationReason { get; set; }

        /// <summary>
        /// Number of nights
        /// </summary>
        public int NightsCount => (CheckOutDate - CheckInDate).Days;
    }
}