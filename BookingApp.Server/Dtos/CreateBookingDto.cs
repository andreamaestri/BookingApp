using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// DTO for creating a new booking
    /// </summary>
    public class CreateBookingDto
    {
        /// <summary>
        /// Accommodation ID to book
        /// </summary>
        [Required]
        public int AccommodationId { get; set; }
        
        /// <summary>
        /// Check-in date
        /// </summary>
        [Required]
        public DateTime CheckInDate { get; set; }
        
        /// <summary>
        /// Check-out date
        /// </summary>
        [Required]
        public DateTime CheckOutDate { get; set; }
        
        /// <summary>
        /// Number of guests
        /// </summary>
        [Required]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20")]
        public int GuestCount { get; set; }
        
        /// <summary>
        /// Special requests from the guest
        /// </summary>
        [MaxLength(500, ErrorMessage = "Special requests cannot exceed 500 characters")]
        public string SpecialRequests { get; set; }
    }
}