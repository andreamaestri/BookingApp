using BookingApp.Server.Dtos;
using BookingApp.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        /// <summary>
        /// Gets a paginated list of bookings
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<BookingSummaryDto>), 200)]
        public async Task<IActionResult> GetBookings([FromQuery] BookingFilter filter)
        {
            var bookings = await _bookingRepository.GetBookingsAsync(filter);
            return Ok(bookings);
        }

        /// <summary>
        /// Gets a booking by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookingDetailDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookingDetailDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createDto, [FromQuery] int guestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var booking = await _bookingRepository.CreateBookingAsync(createDto, guestId);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookingRepository.UpdateBookingAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancels a booking
        /// </summary>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelBooking(int id, [FromBody] CancellationDto cancellationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookingRepository.CancelBookingAsync(id, cancellationDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets all bookings for a specific guest
        /// </summary>
        [HttpGet("guest/{guestId}")]
        [ProducesResponseType(typeof(List<BookingSummaryDto>), 200)]
        public async Task<IActionResult> GetBookingsByGuestId(int guestId)
        {
            var bookings = await _bookingRepository.GetBookingsByGuestIdAsync(guestId);
            return Ok(bookings);
        }

        /// <summary>
        /// Gets all bookings for a specific accommodation
        /// </summary>
        [HttpGet("accommodation/{accommodationId}")]
        [ProducesResponseType(typeof(List<BookingSummaryDto>), 200)]
        public async Task<IActionResult> GetBookingsByAccommodationId(int accommodationId)
        {
            var bookings = await _bookingRepository.GetBookingsByAccommodationIdAsync(accommodationId);
            return Ok(bookings);
        }

        /// <summary>
        /// Checks if an accommodation is available for the specified dates
        /// </summary>
        [HttpGet("check-availability")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CheckAvailability([FromQuery] int accommodationId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            if (checkIn >= checkOut)
            {
                return BadRequest("Check-out date must be after check-in date");
            }

            var isAvailable = await _bookingRepository.IsAccommodationAvailableAsync(accommodationId, checkIn, checkOut);
            return Ok(isAvailable);
        }
    }
}