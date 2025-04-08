import { useState, useEffect } from 'react';
import axios from 'axios';
import './BookingList.css';

const BookingList = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState({
    status: '',
    pageNumber: 1,
    pageSize: 10
  });

  useEffect(() => {
    fetchBookings();
  }, [filter]);

  const fetchBookings = async () => {
    try {
      setLoading(true);
      // Build query parameters
      const params = new URLSearchParams();
      if (filter.status) params.append('status', filter.status);
      params.append('pageNumber', filter.pageNumber);
      params.append('pageSize', filter.pageSize);
      
      const response = await axios.get(`/api/v1/bookings?${params.toString()}`);
      // Check if response.data has the expected structure
      if (response.data && response.data.items) {
        setBookings(response.data.items);
      } else {
        // If items is not present, use the whole response.data if it's an array, otherwise use empty array
        setBookings(Array.isArray(response.data) ? response.data : []);
      }
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch bookings. Please try again later.');
      setLoading(false);
      console.error('Error fetching bookings:', err);
    }
  };

  const handleStatusChange = (e) => {
    setFilter({
      ...filter,
      status: e.target.value,
      pageNumber: 1 // Reset to first page when filter changes
    });
  };

  const handlePageChange = (newPage) => {
    setFilter({
      ...filter,
      pageNumber: newPage
    });
  };

  const getStatusClass = (status) => {
    switch (status) {
      case 0: // Pending
        return 'status-pending';
      case 1: // Confirmed
        return 'status-confirmed';
      case 2: // Cancelled
        return 'status-cancelled';
      case 3: // Completed
        return 'status-completed';
      default:
        return '';
    }
  };

  const getStatusText = (status) => {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Confirmed';
      case 2: return 'Cancelled';
      case 3: return 'Completed';
      default: return 'Unknown';
    }
  };

  const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  if (loading) return <div className="loading">Loading bookings...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="booking-list-container">
      <h2>Your Bookings</h2>
      
      <div className="filter-controls">
        <label>
          Filter by status:
          <select value={filter.status} onChange={handleStatusChange}>
            <option value="">All Statuses</option>
            <option value="0">Pending</option>
            <option value="1">Confirmed</option>
            <option value="2">Cancelled</option>
            <option value="3">Completed</option>
          </select>
        </label>
      </div>

      {bookings.length === 0 ? (
        <div className="no-bookings">No bookings found</div>
      ) : (
        <>
          <div className="bookings-grid">
            {bookings.map(booking => (
              <div key={booking.id} className="booking-card">
                <div className="booking-header">
                  <h3>{booking.accommodationName}</h3>
                  <span className={`booking-status ${getStatusClass(booking.status)}`}>
                    {getStatusText(booking.status)}
                  </span>
                </div>
                <div className="booking-details">
                  <div className="booking-dates">
                    <div className="date-group">
                      <span className="date-label">Check-in</span>
                      <span className="date-value">{formatDate(booking.checkInDate)}</span>
                    </div>
                    <div className="date-group">
                      <span className="date-label">Check-out</span>
                      <span className="date-value">{formatDate(booking.checkOutDate)}</span>
                    </div>
                  </div>
                  <div className="booking-info">
                    <p><strong>Booking ID:</strong> {booking.id}</p>
                    <p><strong>Nights:</strong> {booking.nightsCount}</p>
                    <p><strong>Total:</strong> ${booking.totalPrice.toFixed(2)}</p>
                    <p><strong>Booked on:</strong> {formatDate(booking.bookingDate)}</p>
                  </div>
                </div>
                <div className="booking-actions">
                  <button className="btn-details">View Details</button>
                  {booking.status === 1 && ( // Only show if Confirmed
                    <button className="btn-cancel">Cancel Booking</button>
                  )}
                </div>
              </div>
            ))}
          </div>

          <div className="pagination">
            <button 
              onClick={() => handlePageChange(filter.pageNumber - 1)}
              disabled={filter.pageNumber <= 1}
            >
              Previous
            </button>
            <span>Page {filter.pageNumber}</span>
            <button 
              onClick={() => handlePageChange(filter.pageNumber + 1)}
              disabled={bookings.length < filter.pageSize}
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default BookingList;