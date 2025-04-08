using BookingApp.Server.Model;
using BookingApp.Server.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Server.Repositories
{
    public interface IGuestRepository
    {
        Task<IEnumerable<GuestModel>> GetAllGuestsAsync();
        Task<GuestModel> GetGuestByIdAsync(int id);
        Task<GuestModel> AddGuestAsync(GuestModel guest);
        Task<bool> UpdateGuestAsync(GuestModel guest);
        Task<bool> DeleteGuestAsync(int id);
    }
}