using BookingApp.Server.Model;
using System;

namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// Summary DTO for bookings, used in list operations
    /// </summary>
    public class BookingSummaryDto
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
        /// Accommodation name
        /// </summary>
        public string AccommodationName { get; set; }
        
        /// <summary>
        /// Guest ID
        /// </summary>
        public int GuestId { get; set; }
        
        /// <summary>
        /// Guest name
        /// </summary>
        public string GuestName { get; set; }
        
        /// <summary>
        /// Check-in date
        /// </summary>
        public DateTime CheckInDate { get; set; }
        
        /// <summary>
        /// Check-out date
        /// </summary>
        public DateTime CheckOutDate { get; set; }
        
        /// <summary>
        /// Total price for the booking
        /// </summary>
        public decimal TotalPrice { get; set; }
        
        /// <summary>
        /// Booking status
        /// </summary>
        public BookingStatus Status { get; set; }
        
        /// <summary>
        /// Number of nights
        /// </summary>
        public int NightsCount => (CheckOutDate - CheckInDate).Days;
        
        /// <summary>
        /// Date when the booking was made
        /// </summary>
        public DateTime BookingDate { get; set; }
    }
}