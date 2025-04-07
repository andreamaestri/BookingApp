using BookingApp.Server.Dtos;
using BookingApp.Server.Model;

namespace BookingApp.Server.Repositories
{
    /// <summary>
    /// Repository interface for booking data access operations
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Gets a paginated list of bookings based on filter criteria
        /// </summary>
        Task<Core.PagedResult<BookingSummaryDto>> GetBookingsAsync(BookingFilter filter);

        /// <summary>
        /// Gets detailed information for a specific booking by ID
        /// </summary>
        Task<BookingDetailDto> GetBookingByIdAsync(int id);

        /// <summary>
        /// Gets the database entity for a specific booking by ID
        /// </summary>
        Task<BookingModel> GetBookingEntityAsync(int id);

        /// <summary>
        /// Creates a new booking
        /// </summary>
        Task<BookingDetailDto> CreateBookingAsync(CreateBookingDto createDto, int guestId);

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        Task UpdateBookingAsync(int id, UpdateBookingDto updateDto);

        /// <summary>
        /// Cancels a booking
        /// </summary>
        Task CancelBookingAsync(int id, CancellationDto cancellationDto);

        /// <summary>
        /// Checks if a booking exists by ID
        /// </summary>
        Task<bool> BookingExistsAsync(int id);

        /// <summary>
        /// Checks if the specified accommodation is available for the requested dates
        /// </summary>
        Task<bool> IsAccommodationAvailableAsync(int accommodationId, DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// Gets all bookings for a specific guest
        /// </summary>
        Task<List<BookingSummaryDto>> GetBookingsByGuestIdAsync(int guestId);

        /// <summary>
        /// Gets all bookings for a specific accommodation
        /// </summary>
        Task<List<BookingSummaryDto>> GetBookingsByAccommodationIdAsync(int accommodationId);
    }
}