using BookingApp.Server.Dtos;
using BookingApp.Server.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Server.Repositories
{
    /// <summary>
    /// Implementation of the booking repository for data access operations
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookingRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async Task<PagedResult<BookingSummaryDto>> GetBookingsAsync(BookingFilter filter)
        {
            var query = _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Accommodation)
                .AsQueryable();

            // Apply filters
            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(b => b.CheckInDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(b => b.CheckOutDate <= filter.ToDate.Value);
            }

            // Calculate total items and apply pagination
            var totalItems = await query.CountAsync();
            var pageSize = filter.PageSize ?? 10;
            var pageNumber = filter.PageNumber ?? 1;
            
            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var bookingDtos = _mapper.Map<List<BookingSummaryDto>>(bookings);

            return new PagedResult<BookingSummaryDto>
            {
                Items = bookingDtos,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <inheritdoc />
        public async Task<BookingDetailDto> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Accommodation)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return null;
            }

            return _mapper.Map<BookingDetailDto>(booking);
        }

        /// <inheritdoc />
        public async Task<BookingModel> GetBookingEntityAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Accommodation)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <inheritdoc />
        public async Task<BookingDetailDto> CreateBookingAsync(CreateBookingDto createDto, int guestId)
        {
            // Check if accommodation exists
            var accommodation = await _context.Accommodations.FindAsync(createDto.AccommodationId);
            if (accommodation == null)
            {
                throw new KeyNotFoundException($"Accommodation with ID {createDto.AccommodationId} not found.");
            }

            // Check if guest exists
            var guest = await _context.Guests.FindAsync(guestId);
            if (guest == null)
            {
                throw new KeyNotFoundException($"Guest with ID {guestId} not found.");
            }

            // Check if accommodation is available for the requested dates
            bool isAvailable = await IsAccommodationAvailableAsync(
                createDto.AccommodationId, 
                createDto.CheckInDate, 
                createDto.CheckOutDate);

            if (!isAvailable)
            {
                throw new InvalidOperationException("The accommodation is not available for the requested dates.");
            }

            // Calculate total price (nights × price per night)
            var nights = (createDto.CheckOutDate - createDto.CheckInDate).Days;
            var totalPrice = accommodation.PricePerNight * nights;

            // Create new booking
            var booking = new BookingModel
            {
                AccommodationId = createDto.AccommodationId,
                GuestId = guestId,
                CheckInDate = createDto.CheckInDate,
                CheckOutDate = createDto.CheckOutDate,
                GuestCount = createDto.GuestCount,
                Status = BookingStatus.Confirmed,
                TotalPrice = totalPrice,
                BookingDate = DateTime.UtcNow,
                SpecialRequests = createDto.SpecialRequests
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return await GetBookingByIdAsync(booking.Id);
        }

        /// <inheritdoc />
        public async Task UpdateBookingAsync(int id, UpdateBookingDto updateDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found.");
            }

            // Check if dates are being changed and if so, verify availability
            if (updateDto.CheckInDate.HasValue && updateDto.CheckOutDate.HasValue &&
                (booking.CheckInDate != updateDto.CheckInDate || booking.CheckOutDate != updateDto.CheckOutDate))
            {
                bool isAvailable = await IsAccommodationAvailableAsync(
                    booking.AccommodationId,
                    updateDto.CheckInDate.Value,
                    updateDto.CheckOutDate.Value,
                    id);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("The accommodation is not available for the requested dates.");
                }

                // Recalculate total price if dates changed
                var accommodation = await _context.Accommodations.FindAsync(booking.AccommodationId);
                var nights = (updateDto.CheckOutDate.Value - updateDto.CheckInDate.Value).Days;
                booking.TotalPrice = accommodation.PricePerNight * nights;
            }

            // Update booking properties
            if (updateDto.CheckInDate.HasValue)
                booking.CheckInDate = updateDto.CheckInDate.Value;
            
            if (updateDto.CheckOutDate.HasValue)
                booking.CheckOutDate = updateDto.CheckOutDate.Value;
            
            if (updateDto.GuestCount.HasValue)
                booking.GuestCount = updateDto.GuestCount.Value;
            
            if (updateDto.Status.HasValue)
                booking.Status = updateDto.Status.Value;
            
            if (updateDto.SpecialRequests != null)
                booking.SpecialRequests = updateDto.SpecialRequests;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task CancelBookingAsync(int id, CancellationDto cancellationDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = cancellationDto.Reason;
            booking.CancellationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> BookingExistsAsync(int id)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }

        /// <inheritdoc />
        public async Task<bool> IsAccommodationAvailableAsync(int accommodationId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            // First check if the accommodation exists
            var accommodationExists = await _context.Accommodations.AnyAsync(a => a.Id == accommodationId);
            if (!accommodationExists)
            {
                return false;
            }

            // Check if the requested dates are valid
            if (checkIn >= checkOut || checkIn < DateTime.Today)
            {
                return false;
            }

            // Check if accommodation has available periods that cover the requested dates
            var hasAvailablePeriod = await _context.AvailabilityPeriods
                .AnyAsync(p => p.AccommodationId == accommodationId 
                            && p.StartDate <= checkIn 
                            && p.EndDate >= checkOut);

            if (!hasAvailablePeriod)
            {
                return false;
            }

            // Check for overlapping bookings
            var query = _context.Bookings
                .Where(b => b.AccommodationId == accommodationId
                       && b.Status != BookingStatus.Cancelled
                       && ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||  // Booking starts before requested check-in and ends after
                          (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) || // Booking starts before requested check-out and ends after
                          (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut))); // Booking is entirely within requested period

            // Exclude the current booking if we're updating
            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            // If any overlapping bookings exist, the accommodation is not available
            var hasOverlappingBookings = await query.AnyAsync();
            return !hasOverlappingBookings;
        }

        /// <inheritdoc />
        public async Task<List<BookingSummaryDto>> GetBookingsByGuestIdAsync(int guestId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Accommodation)
                .Where(b => b.GuestId == guestId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return _mapper.Map<List<BookingSummaryDto>>(bookings);
        }

        /// <inheritdoc />
        public async Task<List<BookingSummaryDto>> GetBookingsByAccommodationIdAsync(int accommodationId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Guest)
                .Where(b => b.AccommodationId == accommodationId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return _mapper.Map<List<BookingSummaryDto>>(bookings);
        }
    }
}