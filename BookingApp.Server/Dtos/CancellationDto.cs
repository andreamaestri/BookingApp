namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// DTO for booking cancellation
    /// </summary>
    public class CancellationDto
    {
        /// <summary>
        /// Reason for cancelling the booking
        /// </summary>
        [Required]
        [MaxLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters")]
        public string Reason { get; set; }
    }
}