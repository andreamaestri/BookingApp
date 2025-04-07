using BookingApp.Server.Model;
using System;

namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// Filter options for retrieving bookings
    /// </summary>
    public class BookingFilter
    {
        /// <summary>
        /// Filter by booking status
        /// </summary>
        public BookingStatus? Status { get; set; }
        
        /// <summary>
        /// Filter by check-in date (from)
        /// </summary>
        public DateTime? FromDate { get; set; }
        
        /// <summary>
        /// Filter by check-out date (to)
        /// </summary>
        public DateTime? ToDate { get; set; }
        
        /// <summary>
        /// Page number for pagination
        /// </summary>
        public int? PageNumber { get; set; }
        
        /// <summary>
        /// Page size for pagination
        /// </summary>
        public int? PageSize { get; set; }
        
        /// <summary>
        /// Filter by guest ID
        /// </summary>
        public int? GuestId { get; set; }
        
        /// <summary>
        /// Filter by accommodation ID
        /// </summary>
        public int? AccommodationId { get; set; }
    }
}