using AutoMapper;
using BookingApp.Server.Core;
using BookingApp.Server.Dtos;
using BookingApp.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // Specifies the default content type for responses
    public class AccommodationController : ControllerBase
    {
        // --- Dependencies ---
        private readonly IAccommodationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccommodationController> _logger;

        // --- Constants ---
        // Define constants for configuration values or magic numbers to improve readability and maintainability.
        // Using constants avoids scattering literal values throughout the code.
        private const int DefaultPageSize = 10; // Default if not specified or invalid
        private const int MaxPageSize = 50;     // Upper limit to prevent resource exhaustion

        // --- Constructor ---
        // Use dependency injection (DI) to provide instances of required services.
        // This promotes loose coupling and testability.
        // Null checks in the constructor ensure essential dependencies are available at startup.
        public AccommodationController(
            IAccommodationRepository repository,
            IMapper mapper,
            ILogger<AccommodationController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === GET Endpoints ===

        /// <summary>
        /// Retrieves a paginated and filtered list of accommodation summaries.
        /// </summary>
        /// <remarks>
        /// Supports filtering by various criteria defined in <see cref="AccommodationFilter"/>.
        /// Pagination defaults and limits are applied to ensure efficient data retrieval.
        /// Anonymous access is allowed for browsing accommodations.
        /// </remarks>
        /// <param name="filter">Query parameters for filtering, sorting, and pagination.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResult{AccommodationSummaryDto}"/> containing the accommodations.</returns>
        /// <response code="200">Returns the paginated list of accommodation summaries.</response>
        /// <response code="400">If the filter parameters are invalid (e.g., negative page number).</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpGet]
        [AllowAnonymous] // Publicly accessible endpoint
        [ProducesResponseType(typeof(Core.PagedResult<AccommodationSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Core.PagedResult<AccommodationSummaryDto>>> GetAccommodations(
            [FromQuery] AccommodationFilter filter,
            CancellationToken cancellationToken) // Added CancellationToken
        {
            try
            {
                // --- Input Validation & Normalization ---
                // Clamp page size to reasonable limits to prevent abuse and ensure performance.
                // Ensure page number is positive.
                // Consider moving more complex validation to a dedicated filter/validator or the DTO itself.
                filter.PageSize = Math.Clamp(filter.PageSize > 0 ? filter.PageSize : DefaultPageSize, 1, MaxPageSize);
                filter.PageNumber = Math.Max(1, filter.PageNumber);

                _logger.LogInformation("Attempting to retrieve accommodations with filter: {@Filter}", filter);

                // Delegate the core logic to the repository layer.
                var result = await _repository.GetAccommodationsAsync(filter);

                _logger.LogInformation("Successfully retrieved {Count} accommodations on page {PageNumber} with page size {PageSize}",
                    result.Items.Count(), filter.PageNumber, filter.PageSize);

                return Ok(result);
            }
            // Specific exception handling can be added here if needed (e.g., ArgumentException for bad filters).
            catch (Exception ex)
            {
                // --- Error Handling & Logging ---
                // Log errors with structured data for better analysis. Include TraceIdentifier for correlation.
                _logger.LogError(ex, "An error occurred while retrieving accommodations. Filter: {@Filter}, TraceId: {TraceId}",
                                 filter, HttpContext.TraceIdentifier);

                // Return a standardized error response (RFC 7807 ProblemDetails).
                return Problem(
                    detail: "An internal error occurred while processing your request. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Retrieval Error");
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific accommodation by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the accommodation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The <see cref="AccommodationDetailDto"/> for the requested accommodation.</returns>
        /// <response code="200">Returns the accommodation details.</response>
        /// <response code="404">If an accommodation with the specified ID is not found.</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpGet("{id:int}", Name = "GetAccommodationById")] // Added route name for CreatedAtAction
        [AllowAnonymous]
        [ProducesResponseType(typeof(AccommodationDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccommodationDetailDto>> GetAccommodation(int id, CancellationToken cancellationToken)
        {
            // Basic input validation (ID constraints can be added via routing if needed).
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid ID", Detail = "Accommodation ID must be positive.", Status = StatusCodes.Status400BadRequest });
            }

            try
            {
                _logger.LogInformation("Attempting to retrieve accommodation details for ID {AccommodationId}", id);

                // Fetch data via repository. The repository should handle mapping if it returns DTOs directly.
                var accommodationDto = await _repository.GetAccommodationByIdAsync(id);

                if (accommodationDto == null)
                {
                    _logger.LogWarning("Accommodation with ID {AccommodationId} not found.", id);
                    // Return a standard 404 response.
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found.", Status = StatusCodes.Status404NotFound });
                }

                _logger.LogInformation("Successfully retrieved accommodation details for ID {AccommodationId}", id);
                return Ok(accommodationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accommodation details for ID {AccommodationId}. TraceId: {TraceId}",
                                 id, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "An internal error occurred while retrieving accommodation details.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Retrieval Error");
            }
        }

        // === POST Endpoint ===

        /// <summary>
        /// Creates a new accommodation listing.
        /// </summary>
        /// <remarks>
        /// Requires authentication and authorization (Admin or Owner role).
        /// The owner ID is automatically assigned based on the authenticated user.
        /// </remarks>
        /// <param name="createDto">Data Transfer Object containing the details for the new accommodation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The newly created accommodation details (<see cref="AccommodationDetailDto"/>).</returns>
        /// <response code="201">Returns the newly created accommodation and its location header.</response>
        /// <response code="400">If the request body is invalid or fails validation.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the authenticated user does not have the required role (Admin or Owner).</response>
        /// <response code="500">If an unexpected server error occurs during creation.</response>
        [HttpPost]
        [Authorize(Roles = "Admin, Owner")] // Secure endpoint
        [ProducesResponseType(typeof(AccommodationDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)] // Use ValidationProblemDetails for model state errors
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccommodationDetailDto>> CreateAccommodation(
            [FromBody] CreateAccommodationDto createDto,
            CancellationToken cancellationToken)
        {
            // ModelState validation is handled automatically by [ApiController] attribute for [FromBody] parameters.

            try
            {
                // Extract user ID from claims. Handle potential issues gracefully.
                int? ownerId = GetCurrentUserIdOrDefault();
                if (ownerId == null)
                {
                    // This indicates a potential issue with token validation or claim setup.
                    _logger.LogWarning("Attempted to create accommodation but user ID claim was missing or invalid. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                    return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID could not be determined from token.", Status = StatusCodes.Status401Unauthorized });
                }

                _logger.LogInformation("User {UserId} attempting to create accommodation: {@CreateAccommodationDto}", ownerId.Value, createDto);

                // Delegate creation logic to the repository/service layer.
                var createdDto = await _repository.CreateAccommodationAsync(createDto, ownerId.Value);

                _logger.LogInformation("Accommodation {AccommodationId} created successfully by User {UserId}", createdDto.Id, ownerId.Value);

                // Return 201 Created with the Location header pointing to the newly created resource
                // and include the created resource in the body. Use the route name defined in the GET endpoint.
                return CreatedAtRoute("GetAccommodationById", new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex) // Consider catching more specific exceptions from the repository if applicable (e.g., validation errors).
            {
                _logger.LogError(ex, "Error creating accommodation by User {UserId}. DTO: {@CreateAccommodationDto}, TraceId: {TraceId}",
                                 GetCurrentUserIdOrDefault(), createDto, HttpContext.TraceIdentifier); // Log user ID if available
                return Problem(
                    detail: "An error occurred while creating the accommodation.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Creation Error");
            }
        }

        // === PUT Endpoint ===

        /// <summary>
        /// Updates an existing accommodation entirely. Replaces the resource at the given ID.
        /// </summary>
        /// <remarks>
        /// Requires authentication and the user must be an Admin or the Owner of the accommodation.
        /// Performs a full update; missing fields in the DTO might null out corresponding entity fields if applicable.
        /// </remarks>
        /// <param name="id">The ID of the accommodation to update.</param>
        /// <param name="updateDto">The DTO containing the updated data for the accommodation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success (204 No Content) or failure.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the request body is invalid or fails validation.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized (not Admin or Owner).</response>
        /// <response code="404">If the accommodation with the specified ID is not found.</response>
        /// <response code="409">If a concurrency conflict occurs during the update.</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, Owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAccommodation(
            int id,
            [FromBody] UpdateAccommodationDto updateDto,
            CancellationToken cancellationToken)
        {
            // Basic ID validation
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid ID", Detail = "Accommodation ID must be positive.", Status = StatusCodes.Status400BadRequest });
            }
            // Consider adding `if (id != updateDto.Id)` check if the ID is part of the DTO, though it's often omitted in PUT DTOs.

            try
            {
                // --- Authorization Check ---
                // Fetch the entity minimally needed for the ownership check.
                // Avoid fetching the full entity if only the OwnerId is needed for authorization,
                // although GetAccommodationEntityAsync might already be optimized.
                var accommodation = await _repository.GetAccommodationEntityAsync(id);
                if (accommodation == null)
                {
                    _logger.LogWarning("Update failed: Accommodation with ID {AccommodationId} not found.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found.", Status = StatusCodes.Status404NotFound });
                }

                // Perform authorization check: User must be Admin or the owner.
                if (!await IsAdminOrOwnerAsync(accommodation.OwnerId))
                {
                    _logger.LogWarning("User {UserId} attempted forbidden PUT on accommodation {AccommodationId} owned by {OwnerId}. TraceId: {TraceId}",
                                       GetCurrentUserIdOrDefault(), id, accommodation.OwnerId, HttpContext.TraceIdentifier);
                    return Forbid(); // Returns 403 Forbidden
                }

                _logger.LogInformation("User {UserId} attempting to PUT update accommodation {AccommodationId}", GetCurrentUserIdOrDefault(), id);

                // --- Perform Update ---
                // Delegate the update logic to the repository.
                // The repository should handle mapping from DTO to entity and saving changes.
                await _repository.UpdateAccommodationAsync(id, updateDto);

                // Note: UpdateAccommodationAsync returning bool might be less common than throwing exceptions.
                // If it returns false when not found (after the initial check), handle appropriately.
                // Assuming here it throws DbUpdateConcurrencyException or similar on issues post-authorization.

                _logger.LogInformation("Accommodation {AccommodationId} updated successfully via PUT by User {UserId}", id, GetCurrentUserIdOrDefault());

                // Return 204 No Content on successful update.
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // --- Concurrency Handling ---
                _logger.LogWarning(ex, "Concurrency conflict occurred while attempting to PUT update accommodation ID {AccommodationId}. TraceId: {TraceId}",
                                   id, HttpContext.TraceIdentifier);

                // Optional: Check if the entity still exists after the conflict.
                if (!await _repository.AccommodationExistsAsync(id)) // Use CancellationToken.None for this quick check
                {
                    _logger.LogWarning("Concurrency conflict for accommodation ID {AccommodationId}, but it no longer exists.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found (possibly deleted).", Status = StatusCodes.Status404NotFound });
                }
                else
                {
                    // Return 409 Conflict if it still exists but couldn't be updated due to concurrency.
                    return Conflict(new ProblemDetails
                    {
                        Title = "Concurrency Conflict",
                        Detail = "The accommodation was modified by another user after you loaded it. Please refresh and try your update again.",
                        Status = StatusCodes.Status409Conflict
                    });
                }
            }
            catch (Exception ex) // Catch other potential exceptions from the repository or mapping.
            {
                _logger.LogError(ex, "Error PUT updating accommodation ID {AccommodationId}. DTO: {@UpdateAccommodationDto}, TraceId: {TraceId}",
                                 id, updateDto, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "An error occurred while updating the accommodation.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Update Error");
            }
        }

        // === PATCH Endpoint ===

        /// <summary>
        /// Partially updates an existing accommodation using JSON Patch standard.
        /// </summary>
        /// <remarks>
        /// Requires authentication and the user must be an Admin or the Owner.
        /// Applies specific changes outlined in the patch document without replacing the entire resource.
        /// Operations like replacing sensitive fields (e.g., OwnerId) should be validated or disallowed if necessary.
        /// </remarks>
        /// <param name="id">The ID of the accommodation to patch.</param>
        /// <param name="patchDoc">The JSON Patch document (<see cref="JsonPatchDocument{UpdateAccommodationDto}"/>) specifying the operations.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success (204 No Content) or failure.</returns>
        /// <response code="204">If the patch was applied successfully.</response>
        /// <response code="400">If the patch document is null, invalid, or results in an invalid model state.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized (not Admin or Owner).</response>
        /// <response code="404">If the accommodation with the specified ID is not found.</response>
        /// <response code="409">If a concurrency conflict occurs during the update (less common with PATCH but possible).</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin, Owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)] // Includes patch validation errors
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // Keep, just in case
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PartiallyUpdateAccommodation(
            int id,
            [FromBody] JsonPatchDocument<UpdateAccommodationDto> patchDoc,
            CancellationToken cancellationToken)
        {
            // Basic ID validation
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid ID", Detail = "Accommodation ID must be positive.", Status = StatusCodes.Status400BadRequest });
            }

            // Validate the patch document itself isn't null
            if (patchDoc == null)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid Input", Detail = "A JSON Patch document is required.", Status = StatusCodes.Status400BadRequest });
            }

            try
            {
                // --- Fetch and Authorize ---
                // Fetch the entity. We need it to apply the patch correctly and check ownership.
                var accommodation = await _repository.GetAccommodationEntityAsync(id);
                if (accommodation == null)
                {
                    _logger.LogWarning("PATCH failed: Accommodation with ID {AccommodationId} not found.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found.", Status = StatusCodes.Status404NotFound });
                }

                // Authorization check
                if (!await IsAdminOrOwnerAsync(accommodation.OwnerId))
                {
                    _logger.LogWarning("User {UserId} attempted forbidden PATCH on accommodation {AccommodationId} owned by {OwnerId}. TraceId: {TraceId}",
                                       GetCurrentUserIdOrDefault(), id, accommodation.OwnerId, HttpContext.TraceIdentifier);
                    return Forbid();
                }

                _logger.LogInformation("User {UserId} attempting to PATCH accommodation {AccommodationId}", GetCurrentUserIdOrDefault(), id);

                // --- Apply Patch ---
                // 1. Map the existing entity to the DTO that the patch document targets.
                //    This creates a temporary DTO state representing the current resource.
                var accommodationToPatch = _mapper.Map<UpdateAccommodationDto>(accommodation);

                // 2. Apply the patch operations to the temporary DTO.
                //    The `ModelState` parameter allows capturing errors during the application phase (e.g., invalid path).
                patchDoc.ApplyTo(accommodationToPatch);

                // 3. Check if applying the patch caused any errors (e.g., invalid path, invalid value type).
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid PATCH document for accommodation ID {AccommodationId}. ModelState: {@ModelState}", id, ModelState);
                    return BadRequest(ModelState); // Returns ValidationProblemDetails
                }

                // 4. Validate the *resulting* DTO using standard model validation rules.
                //    Ensures the state after patching is still valid according to DTO attributes/validators.
                if (!TryValidateModel(accommodationToPatch))
                {
                    _logger.LogWarning("PATCH resulted in invalid model state for accommodation ID {AccommodationId}. ModelState: {@ModelState}", id, ModelState);
                    return BadRequest(ModelState); // Returns ValidationProblemDetails
                }

                // --- Persist Changes ---
                // 5. Delegate persistence to the repository. This method should handle mapping
                //    the *patched DTO* back to the entity and saving. It might need the original entity
                //    for comparison or concurrency checks depending on implementation.
                await _repository.PatchAccommodationAsync(id, accommodationToPatch); // Pass the patched DTO

                _logger.LogInformation("Accommodation {AccommodationId} patched successfully by User {UserId}", id, GetCurrentUserIdOrDefault());

                return NoContent(); // Return 204 No Content on success.
            }
            catch (DbUpdateConcurrencyException ex) // Handle potential concurrency issues, though less likely for PATCH unless specific fields overlap.
            {
                _logger.LogWarning(ex, "Concurrency conflict occurred while attempting to PATCH accommodation ID {AccommodationId}. TraceId: {TraceId}",
                                  id, HttpContext.TraceIdentifier);
                if (!await _repository.AccommodationExistsAsync(id))
                {
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found (possibly deleted).", Status = StatusCodes.Status404NotFound });
                }
                return Conflict(new ProblemDetails
                {
                    Title = "Concurrency Conflict",
                    Detail = "The accommodation was modified concurrently. Please refresh and try your patch again.",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex) // Catch other potential errors (mapping, repository).
            {
                _logger.LogError(ex, "Error PATCH updating accommodation ID {AccommodationId}. TraceId: {TraceId}",
                                 id, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "An error occurred while partially updating the accommodation.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Patch Error");
            }
        }

        // === DELETE Endpoint ===

        /// <summary>
        /// Deletes a specific accommodation by its ID.
        /// </summary>
        /// <remarks>
        /// Requires authentication and the user must be an Admin or the Owner.
        /// This is typically a hard delete, but could be implemented as a soft delete (marking as inactive)
        /// in the repository layer depending on business requirements.
        /// </remarks>
        /// <param name="id">The ID of the accommodation to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success (204 No Content) or failure.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized (not Admin or Owner).</response>
        /// <response code="404">If the accommodation with the specified ID is not found.</response>
        /// <response code="500">If a database error occurs (e.g., foreign key constraints prevent deletion) or an unexpected server error.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, Owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)] // Covers DB errors too
        public async Task<IActionResult> DeleteAccommodation(int id, CancellationToken cancellationToken)
        {
            // Basic ID validation
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid ID", Detail = "Accommodation ID must be positive.", Status = StatusCodes.Status400BadRequest });
            }

            try
            {
                // --- Fetch and Authorize ---
                // Fetch the entity first to check ownership before deleting.
                var accommodation = await _repository.GetAccommodationEntityAsync(id);
                if (accommodation == null)
                {
                    // Logically, if it doesn't exist, the delete operation is idempotent from the client's perspective.
                    // Returning 404 is conventional to indicate the resource wasn't there to delete.
                    _logger.LogWarning("Delete failed: Accommodation with ID {AccommodationId} not found.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found.", Status = StatusCodes.Status404NotFound });
                }

                // Authorization check
                if (!await IsAdminOrOwnerAsync(accommodation.OwnerId))
                {
                    _logger.LogWarning("User {UserId} attempted forbidden DELETE on accommodation {AccommodationId} owned by {OwnerId}. TraceId: {TraceId}",
                                       GetCurrentUserIdOrDefault(), id, accommodation.OwnerId, HttpContext.TraceIdentifier);
                    return Forbid();
                }

                _logger.LogInformation("User {UserId} attempting to DELETE accommodation {AccommodationId}", GetCurrentUserIdOrDefault(), id);

                // --- Perform Deletion ---
                // Delegate deletion to the repository.
                await _repository.DeleteAccommodationAsync(id);

                // Check if deletion was successful (repository might return false if it fails for some reason,
                // though throwing is more common). Assuming here it throws on failure post-authorization.
                // if (!deleted) { ... handle failure ... }

                _logger.LogInformation("Accommodation with ID {AccommodationId} deleted successfully by User {UserId}.", id, GetCurrentUserIdOrDefault());
                return NoContent(); // Standard response for successful DELETE.
            }
            catch (DbUpdateException ex) // Specifically catch database update exceptions
            {
                // This often indicates issues like foreign key constraints (e.g., existing bookings).
                _logger.LogError(ex, "Database error occurred while deleting accommodation ID {AccommodationId}. It might have dependent records (e.g., bookings). TraceId: {TraceId}",
                                 id, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "Could not delete the accommodation. It might be referenced by other data (like bookings or reviews). Please ensure related data is removed first.",
                    statusCode: StatusCodes.Status500InternalServerError, // Could argue for 409 Conflict, but 500 is common for unexpected DB constraints.
                    title: "Deletion Failed");
            }
            catch (Exception ex) // Catch any other unexpected errors.
            {
                _logger.LogError(ex, "Error deleting accommodation with ID {AccommodationId}. TraceId: {TraceId}",
                                 id, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "An error occurred while deleting the accommodation.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Accommodation Deletion Error");
            }
        }


        // === Nested Resource Endpoint (Example: Availability) ===

        /// <summary>
        /// Adds a new availability/pricing period for a specific accommodation.
        /// </summary>
        /// <remarks>
        /// Requires authentication and the user must be an Admin or the Owner of the accommodation.
        /// Validates that the end date is not before the start date.
        /// Consider adding more complex validation (e.g., checking for overlapping periods) either here or in the repository/service layer.
        /// </remarks>
        /// <param name="id">The ID of the accommodation to add availability for.</param>
        /// <param name="periodDto">The details of the availability period to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The created <see cref="AvailabilityPeriodDto"/>.</returns>
        /// <response code="201">Returns the newly created availability period.</response>
        /// <response code="400">If the input DTO is invalid (e.g., end date before start date, overlapping periods if checked).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized (not Admin or Owner).</response>
        /// <response code="404">If the accommodation with the specified ID is not found.</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpPost("{id:int}/availability")]
        [Authorize(Roles = "Admin, Owner")]
        [ProducesResponseType(typeof(AvailabilityPeriodDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AvailabilityPeriodDto>> AddAvailabilityPeriod(
            int id,
            [FromBody] AvailabilityPeriodDto periodDto,
            CancellationToken cancellationToken)
        {
            // Basic ID validation
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid ID", Detail = "Accommodation ID must be positive.", Status = StatusCodes.Status400BadRequest });
            }

            // --- Basic DTO Validation ---
            // More complex validation (like overlapping checks) might belong in a service/repository or use FluentValidation.
            if (periodDto.EndDate < periodDto.StartDate)
            {
                // Add error to ModelState for a standard ValidationProblemDetails response.
                ModelState.AddModelError(nameof(periodDto.EndDate), "End date cannot be before the start date.");
                _logger.LogWarning("Invalid availability period DTO for accommodation {AccommodationId}: End date before start date. DTO: {@AvailabilityPeriodDto}", id, periodDto);
                return BadRequest(ModelState);
            }

            // Fix: Use PricePerNightOverride instead of non-existent PricePerNight
            if (periodDto.PricePerNightOverride.HasValue && periodDto.PricePerNightOverride.Value <= 0)
            {
                ModelState.AddModelError(nameof(periodDto.PricePerNightOverride), "Price per night must be positive.");
                _logger.LogWarning("Invalid availability period DTO for accommodation {AccommodationId}: Non-positive price. DTO: {@AvailabilityPeriodDto}", id, periodDto);
                return BadRequest(ModelState);
            }

            try
            {
                // --- Fetch and Authorize ---
                // Verify the accommodation exists and check ownership.
                var accommodation = await _repository.GetAccommodationEntityAsync(id);
                if (accommodation == null)
                {
                    _logger.LogWarning("Add availability failed: Accommodation with ID {AccommodationId} not found.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Accommodation with ID {id} not found.", Status = StatusCodes.Status404NotFound });
                }

                if (!await IsAdminOrOwnerAsync(accommodation.OwnerId))
                {
                    _logger.LogWarning("User {UserId} attempted forbidden POST to availability for accommodation {AccommodationId} owned by {OwnerId}. TraceId: {TraceId}",
                                       GetCurrentUserIdOrDefault(), id, accommodation.OwnerId, HttpContext.TraceIdentifier);
                    return Forbid();
                }

                _logger.LogInformation("User {UserId} attempting to add availability period to accommodation {AccommodationId}. Period: {@AvailabilityPeriodDto}",
                                       GetCurrentUserIdOrDefault(), id, periodDto);

                // --- Add Period ---
                // Delegate the actual addition logic (including overlap checks if implemented) to the repository.
                var createdPeriodDto = await _repository.AddAvailabilityPeriodAsync(id, periodDto);

                _logger.LogInformation("Availability period added successfully for accommodation {AccommodationId} by User {UserId}. New Period ID (if applicable): {PeriodId}",
                                       id, GetCurrentUserIdOrDefault(), createdPeriodDto.Id); // Assuming AvailabilityPeriodDto has an Id

                // Return 201 Created. The location header ideally points to the newly created availability period itself,
                // if there's an endpoint to get a specific period. If not, pointing back to the parent accommodation is acceptable.
                // Using GetAccommodationById route name here as an example fallback.
                return CreatedAtRoute("GetAccommodationById", new { id = id }, createdPeriodDto);
                // Alternative if there's a GetAvailabilityPeriod endpoint:
                // return CreatedAtRoute("GetAvailabilityPeriod", new { accommodationId = id, periodId = createdPeriodDto.Id }, createdPeriodDto);

            }
            // Catch specific exceptions related to business rules (e.g., overlapping periods) if thrown by the repository.
            // catch (OverlapException ex) { ... return Conflict(...) ... }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding availability period for accommodation ID {AccommodationId}. DTO: {@AvailabilityPeriodDto}, TraceId: {TraceId}",
                                id, periodDto, HttpContext.TraceIdentifier);
                return Problem(
                    detail: "An error occurred while adding the availability period.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Availability Update Error");
            }
        }


        #region Helper Methods

        /// <summary>
        /// Retrieves the current user's ID from the NameIdentifier claim.
        /// </summary>
        /// <returns>The integer user ID.</returns>
        /// <exception cref="InvalidOperationException">If the claim is missing or not a valid integer.
        /// This indicates a fundamental issue with authentication setup or token contents.</exception>
        private int GetCurrentUserId()
        {
            // Use User.FindFirstValue for convenience.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Robust parsing and error handling are crucial for security-sensitive information.
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                // Log this critical failure. It shouldn't happen in a properly configured system.
                _logger.LogCritical("Could not find or parse User ID claim (NameIdentifier) for the authenticated user. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                // Throwing an exception here is appropriate because subsequent operations requiring the User ID cannot proceed safely.
                // Consider if a specific HTTP status code (e.g., 500 or 401) should be returned higher up if this is recoverable.
                throw new InvalidOperationException("User ID claim (NameIdentifier) is missing or invalid in the security context.");
            }
            return userId;
        }

        /// <summary>
        /// Retrieves the current user's ID from the NameIdentifier claim, returning null if not found or invalid.
        /// Use this variant for logging or contexts where throwing an exception is undesirable.
        /// </summary>
        /// <returns>The integer user ID or null if not found/invalid.</returns>
        private int? GetCurrentUserIdOrDefault()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            _logger.LogWarning("Could not find or parse User ID claim (NameIdentifier) for the authenticated user. TraceId: {TraceId}", HttpContext.TraceIdentifier);
            return null;
        }


        /// <summary>
        /// Checks if the currently authenticated user has the 'Admin' role or if their ID matches the provided owner ID.
        /// Encapsulates the common authorization logic for resource ownership checks.
        /// </summary>
        /// <param name="ownerId">The ID of the resource owner to check against.</param>
        /// <returns>True if the user is an Admin or the owner; otherwise, false.</returns>
        /// <remarks>
        /// This relies on `GetCurrentUserIdOrDefault` internally to handle potential claim issues gracefully
        /// without throwing within the authorization check itself. If the user ID cannot be determined,
        /// ownership check will inherently fail unless they are an Admin.
        /// The `Task<bool>` is kept for potential future async operations within the check, though currently it's synchronous.
        /// </remarks>
        private Task<bool> IsAdminOrOwnerAsync(int ownerId) // Kept async signature for consistency/future use
        {
            // Check for Admin role first. This is often the simplest bypass for ownership checks.
            if (User.IsInRole("Admin"))
            {
                return Task.FromResult(true);
            }

            // Get the current user's ID. Use the null-returning variant for safety within this check.
            int? currentUserId = GetCurrentUserIdOrDefault();

            // If the user ID couldn't be determined, they cannot be the owner.
            // Only proceed if the ID was successfully retrieved.
            bool isOwner = currentUserId.HasValue && currentUserId.Value == ownerId;

            return Task.FromResult(isOwner);
        }

        #endregion
    }
}