using BookingApp.Server.Dtos;
using BookingApp.Server.Model;
using System.Threading.Tasks;

namespace BookingApp.Server.Repositories
{
    /// <summary>
    /// Repository interface for accommodation data access operations
    /// </summary>
    public interface IAccommodationRepository
    {
        /// <summary>
        /// Gets a paginated list of accommodations based on filter criteria
        /// </summary>
        Task<PagedResult<AccommodationSummaryDto>> GetAccommodationsAsync(AccommodationFilter filter);
        
        /// <summary>
        /// Gets detailed information for a specific accommodation by ID
        /// </summary>
        Task<AccommodationDetailDto> GetAccommodationByIdAsync(int id);
        
        /// <summary>
        /// Gets the database entity for a specific accommodation by ID
        /// </summary>
        Task<AccommodationModel> GetAccommodationEntityAsync(int id);
        
        /// <summary>
        /// Creates a new accommodation
        /// </summary>
        Task<AccommodationDetailDto> CreateAccommodationAsync(CreateAccommodationDto createDto, int ownerId);
        
        /// <summary>
        /// Updates an existing accommodation
        /// </summary>
        Task UpdateAccommodationAsync(int id, UpdateAccommodationDto updateDto);
        
        /// <summary>
        /// Partially updates an existing accommodation
        /// </summary>
        Task PatchAccommodationAsync(int id, UpdateAccommodationDto patchDto);
        
        /// <summary>
        /// Deletes an accommodation by ID
        /// </summary>
        Task DeleteAccommodationAsync(int id);
        
        /// <summary>
        /// Checks if an accommodation exists by ID
        /// </summary>
        Task<bool> AccommodationExistsAsync(int id);
        
        /// <summary>
        /// Adds an availability period to an accommodation
        /// </summary>
        Task<AvailabilityPeriodDto> AddAvailabilityPeriodAsync(int accommodationId, AvailabilityPeriodDto periodDto);
    }
}