namespace BookingApp.Server.Dtos
{
    /// <summary>
    /// Summary DTO for guest information
    /// </summary>
    public class GuestSummaryDto
    {
        /// <summary>
        /// Guest ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Guest's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Guest's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Guest's full name (first + last)
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Guest's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Guest's profile picture URL
        /// </summary>
        public string ProfilePictureUrl { get; set; }
    }
}