using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Server.Model
{
    public class GuestModel
    {
        private readonly List<BookingModel> _bookings = new();

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public IReadOnlyCollection<BookingModel> Bookings => _bookings.AsReadOnly();

        internal void AddBooking(BookingModel booking)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));
            _bookings.Add(booking);
        }
    }
}