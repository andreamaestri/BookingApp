using BookingApp.Server.Data;
using BookingApp.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Server.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GuestRepository> _logger;

        public GuestRepository(
            ApplicationDbContext context,
            ILogger<GuestRepository> logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public async Task<IEnumerable<GuestModel>> GetAllGuestsAsync()
        {
            try
            {
                return await _context.Guests.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while retrieving all guests");
                throw;
            }
        }        public async Task<GuestModel> GetGuestByIdAsync(int id)
        {
            try
            {
                return await _context.Guests.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while retrieving guest with ID {GuestId}", id);
                throw;
            }
        }

        public async Task<GuestModel> AddGuestAsync(GuestModel guest)
        {
            try
            {
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();
                return guest;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while adding guest: {@Guest}", guest);
                throw;
            }
        }

        public async Task<bool> UpdateGuestAsync(GuestModel guest)
        {
            try
            {
                _context.Entry(guest).State = EntityState.Modified;
                int result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while updating guest with ID {GuestId}", guest.Id);
                throw;
            }
        }

        public async Task<bool> DeleteGuestAsync(int id)
        {
            try
            {
                var guest = await _context.Guests.FindAsync(id);
                if (guest == null)
                {
                    return false;
                }

                _context.Guests.Remove(guest);
                int result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while deleting guest with ID {GuestId}", id);
                throw;
            }
        }
    }
}