using System.Collections.Generic;

namespace BookingApp.Server.Core
{
    /// <summary>
    /// Generic class for paginated results
    /// </summary>
    /// <typeparam name="T">Type of items in the result set</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Collection of items in the current page
        /// </summary>
        public IList<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        /// <summary>
        /// Indicates if there is a previous page
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates if there is a next page
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;
    }
}