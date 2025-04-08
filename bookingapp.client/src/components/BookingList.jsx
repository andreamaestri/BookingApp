import { useState, useEffect } from 'react';
import axios from 'axios';
import './BookingList.css';

const BookingList = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [apiResponse, setApiResponse] = useState(null);

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Use axios with a specific base URL to ensure proper connection
      const response = await axios.get('https://localhost:7117/api/v1/bookings', {
        params: { pageNumber: 1, pageSize: 10 }
      });
      
      // Store the full response for debugging
      setApiResponse(response.data);
      
      // Check if response.data has the expected structure
      if (response.data && response.data.items) {
        setBookings(response.data.items);
      } else {
        // If items is not present, use the whole response.data if it's an array
        setBookings(Array.isArray(response.data) ? response.data : []);
      }
      setLoading(false);
    } catch (err) {
      console.error('Error details:', err);
      setError(`Failed to fetch bookings: ${err.message}. Status: ${err.response?.status || 'unknown'}`);
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading bookings...</div>;
  
  return (
    <div className="booking-list-container">
      <h2>Your Bookings (Simplified View)</h2>
      
      {error && (
        <div className="error-container">
          <h3>Error</h3>
          <p>{error}</p>
          <button onClick={fetchBookings}>Retry</button>
        </div>
      )}

      {!error && bookings.length === 0 && (
        <div className="no-bookings">
          <p>No bookings found</p>
          <p>API Response: {JSON.stringify(apiResponse, null, 2)}</p>
        </div>
      )}

      {!error && bookings.length > 0 && (
        <div className="bookings-simple-list">
          <h3>Found {bookings.length} bookings:</h3>
          <ul>
            {bookings.map(booking => (
              <li key={booking.id}>
                <strong>ID:</strong> {booking.id} | 
                <strong>Accommodation:</strong> {booking.accommodationName} |
                <strong>Dates:</strong> {new Date(booking.checkInDate).toLocaleDateString()} - {new Date(booking.checkOutDate).toLocaleDateString()}
              </li>
            ))}
          </ul>
          <div className="api-debug">
            <details>
              <summary>API Response Details</summary>
              <pre>{JSON.stringify(apiResponse, null, 2)}</pre>
            </details>
          </div>
        </div>
      )}
    </div>
  );
};

export default BookingList;