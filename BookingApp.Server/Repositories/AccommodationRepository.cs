using AutoMapper;
using BookingApp.Server.Data;
using BookingApp.Server.Dtos;
using BookingApp.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingApp.Server.Repositories
{
    /// <summary>
    /// Implementation of the accommodation repository
    /// </summary>
    public class AccommodationRepository : IAccommodationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AccommodationRepository> _logger;
        
        public AccommodationRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<AccommodationRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<PagedResult<AccommodationSummaryDto>> GetAccommodationsAsync(AccommodationFilter filter)
        {
            try
            {
                var query = _context.Accommodations.AsQueryable();

                // Apply filtering
                query = ApplyFilters(query, filter);

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Get total count (before pagination)
                var totalCount = await query.CountAsync();
                if (totalCount == 0)
                {
                    return new PagedResult<AccommodationSummaryDto>
                    {
                        Items = new List<AccommodationSummaryDto>(),
                        TotalCount = 0,
                        PageNumber = filter.PageNumber,
                        PageSize = filter.PageSize
                    };
                }

                // Instead of direct projection which can cause issues with complex properties,
                // fetch basic entities then map them to DTOs using AutoMapper or manual mapping
                var accommodations = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                // Now manually map to DTOs to avoid EF Core projection issues
                var items = accommodations.Select(a => new AccommodationSummaryDto
                {
                    Id = a.Id,
                    Title = a.Title ?? string.Empty,
                    Type = a.Type,
                    Town = a.Town ?? string.Empty,
                    Bedrooms = a.Bedrooms,
                    MaxOccupancy = a.MaxOccupancy,
                    BasePricePerNight = a.BasePricePerNight,
                    AverageRating = a.AverageRating,
                    HasSeaView = a.HasSeaView,
                    // Safe handling of collections
                    PrimaryImageUrl = a.ImageUrls != null && a.ImageUrls.Any() ? a.ImageUrls.First() : null,
                    IsPetFriendly = a.Amenities != null && a.Amenities.Contains(AmenityType.PetFriendly)
                }).ToList();

                return new PagedResult<AccommodationSummaryDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAccommodationsAsync method");
                throw; // Rethrow to preserve the stack trace
            }
        }

        public async Task<AccommodationDetailDto> GetAccommodationByIdAsync(int id)
        {
            return await _context.Accommodations
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AccommodationDetailDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Type = a.Type,
                    Town = a.Town,
                    Bedrooms = a.Bedrooms,
                    MaxOccupancy = a.MaxOccupancy,
                    BasePricePerNight = a.BasePricePerNight,
                    AverageRating = a.AverageRating,
                    HasSeaView = a.HasSeaView,
                    PrimaryImageUrl = a.ImageUrls != null && a.ImageUrls.Any() ? a.ImageUrls.FirstOrDefault() : null,
                    IsPetFriendly = a.Amenities != null && a.Amenities.Contains(AmenityType.PetFriendly),
                    Description = a.Description,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    PostCode = a.PostCode,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    DistanceToNearestBeachKm = a.DistanceToNearestBeachKm,
                    Bathrooms = a.Bathrooms,
                    CleaningFee = a.CleaningFee,
                    SecurityDeposit = a.SecurityDeposit,
                    OwnerId = a.OwnerId,
                    Amenities = a.Amenities ?? new List<AmenityType>(),
                    ImageUrls = a.ImageUrls ?? new List<string>(),
                    Reviews = a.Reviews != null ? a.Reviews.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ReviewDate = r.ReviewDate,
                        GuestName = (r.Guest != null ? r.Guest.Name : "Anonymous") // Use Name property
                    }).ToList() : new List<ReviewDto>(),
                    AvailabilityPeriods = a.AvailabilityPeriods != null ? a.AvailabilityPeriods.Select(p => new AvailabilityPeriodDto
                    {
                        Id = p.Id,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        PricePerNightOverride = p.PricePerNightOverride,
                        IsAvailable = p.IsAvailable,
                        MinimumStayNights = p.MinimumStayNights,
                        Notes = p.Notes
                    }).ToList() : new List<AvailabilityPeriodDto>(),
                    CreatedDate = a.CreatedDate,
                    LastModifiedDate = a.LastModifiedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AccommodationModel> GetAccommodationEntityAsync(int id)
        {
            return await _context.Accommodations
                .Include(a => a.Amenities)
                .Include(a => a.ImageUrls)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AccommodationDetailDto> CreateAccommodationAsync(CreateAccommodationDto createDto, int ownerId)
        {
            // Map DTO to Model
            var accommodation = _mapper.Map<AccommodationModel>(createDto);

            // Set additional fields
            accommodation.OwnerId = ownerId;
            accommodation.CreatedDate = DateTime.UtcNow;
            accommodation.LastModifiedDate = DateTime.UtcNow;

            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            // Return the created accommodation with details
            return await GetAccommodationByIdAsync(accommodation.Id);
        }

        public async Task UpdateAccommodationAsync(int id, UpdateAccommodationDto updateDto)
        {
            var accommodation = await _context.Accommodations
                .Include(a => a.Amenities)
                .Include(a => a.ImageUrls)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (accommodation == null)
            {
                throw new KeyNotFoundException($"Accommodation with ID {id} not found.");
            }

            // Update properties from DTO
            _mapper.Map(updateDto, accommodation);

            // Update timestamp
            accommodation.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task PatchAccommodationAsync(int id, UpdateAccommodationDto patchDto)
        {
            var accommodation = await _context.Accommodations
                .Include(a => a.Amenities)
                .Include(a => a.ImageUrls)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (accommodation == null)
            {
                throw new KeyNotFoundException($"Accommodation with ID {id} not found.");
            }

            // Update only provided properties
            _mapper.Map(patchDto, accommodation);

            // Update timestamp
            accommodation.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccommodationAsync(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                throw new KeyNotFoundException($"Accommodation with ID {id} not found.");
            }

            _context.Accommodations.Remove(accommodation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AccommodationExistsAsync(int id)
        {
            return await _context.Accommodations.AnyAsync(e => e.Id == id);
        }

        public async Task<AvailabilityPeriodDto> AddAvailabilityPeriodAsync(int accommodationId, AvailabilityPeriodDto periodDto)
        {
            // Map DTO to model
            var period = _mapper.Map<AvailabilityPeriod>(periodDto);
            period.AccommodationId = accommodationId;

            _context.AvailabilityPeriods.Add(period);
            await _context.SaveChangesAsync();

            // Return the created period with its new ID
            return _mapper.Map<AvailabilityPeriodDto>(period);
        }

        #region Helper Methods

        private IQueryable<AccommodationModel> ApplyFilters(IQueryable<AccommodationModel> query, AccommodationFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Town))
            {
                // Safe fallback if PostgreSQL ILike isn't available
                try
                {
                    query = query.Where(a => EF.Functions.Like(a.Town, $"%{filter.Town}%")); // Use SQL Server's Like
                }
                catch (InvalidOperationException)
                {
                    query = query.Where(a => a.Town.ToLower().Contains(filter.Town.ToLower()));
                }
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(a => a.BasePricePerNight >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(a => a.BasePricePerNight <= filter.MaxPrice.Value);

            if (filter.MinBedrooms.HasValue)
                query = query.Where(a => a.Bedrooms >= filter.MinBedrooms.Value);

            if (filter.MinOccupancy.HasValue)
                query = query.Where(a => a.MaxOccupancy >= filter.MinOccupancy.Value);

            if (filter.Type.HasValue)
                query = query.Where(a => a.Type == filter.Type.Value);

            if (filter.RequiredAmenities != null && filter.RequiredAmenities.Any())
            {
                foreach (var amenity in filter.RequiredAmenities)
                {
                    query = query.Where(a => a.Amenities.Contains(amenity));
                }
            }

            if (filter.HasSeaView.HasValue)
                query = query.Where(a => a.HasSeaView == filter.HasSeaView.Value);

            return query;
        }

        private IQueryable<AccommodationModel> ApplySorting(IQueryable<AccommodationModel> query, string sortBy, string sortDirection)
        {
            Expression<Func<AccommodationModel, object>> keySelector = sortBy?.ToLowerInvariant() switch
            {
                "rating" => a => a.AverageRating ?? 0,
                "price" => a => a.BasePricePerNight,
                "name" => a => a.Title,
                "bedrooms" => a => a.Bedrooms,
                _ => a => a.BasePricePerNight
            };

            bool descending = sortDirection?.ToLowerInvariant() == "desc";

            // Create an IOrderedQueryable<T> which supports ThenBy
            var orderedQuery = descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);

            // Add secondary sort for stability
            return orderedQuery.ThenBy(a => a.Id);
        }

        #endregion
    }
}
