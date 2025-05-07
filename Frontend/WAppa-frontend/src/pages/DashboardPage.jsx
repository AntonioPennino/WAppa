// src/pages/DashboardPage.jsx
import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { 
  getFavoriteLocations, 
  addFavoriteLocation, 
  deleteFavoriteLocation 
} from '../services/favoriteLocationService';

// Non abbiamo più bisogno degli stili inline definiti qui
// const cardStyle = { ... };
// const weatherInfoStyle = { ... };
// const italicMessageStyle = { ... };

function DashboardPage() {
  const { currentUser } = useAuth();
  const [favorites, setFavorites] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [newLocationName, setNewLocationName] = useState('');
  const [isAdding, setIsAdding] = useState(false);

  const fetchFavorites = useCallback(async () => {
    setIsLoading(true);
    setError('');
    try {
      const response = await getFavoriteLocations();
      if (response.success) {
        setFavorites(response.data || []);
      } else {
        setError(response.message || 'Failed to load favorites.');
        setFavorites([]);
      }
    } catch (err) {
      setError(err.message || 'An error occurred while fetching favorites.');
      setFavorites([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchFavorites();
  }, [fetchFavorites]);

  const handleAddLocation = async (event) => {
    event.preventDefault();
    if (!newLocationName.trim()) {
      setError('Please enter a location name.');
      return;
    }
    setIsAdding(true);
    setError('');
    try {
      const response = await addFavoriteLocation(newLocationName);
      if (response.success) {
        fetchFavorites(); 
        setNewLocationName('');
      } else {
        setError(response.message || 'Failed to add location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred while adding location.');
    } finally {
      setIsAdding(false);
    }
  };

  const handleDeleteLocation = async (locationId) => {
    setError(''); 
    // Aggiungi qui una conferma se vuoi:
    // if (!window.confirm("Are you sure you want to delete this location?")) return;
    try {
      const response = await deleteFavoriteLocation(locationId);
      if (response.success) {
        fetchFavorites();
      } else {
        setError(response.message || 'Failed to delete location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred while deleting location.');
    }
  };

  if (isLoading) {
    // Potresti usare una classe per lo spinner/messaggio di caricamento
    return <div className="loading-message">Loading your favorite locations...</div>;
  }

  return (
    <div className="dashboard-page-container"> {/* Aggiunta classe contenitore opzionale */}
      <h2>My Dashboard</h2>
      {currentUser && <p>Welcome back, {currentUser.username}!</p>}

      <h3>Add New Favorite Location</h3>
      {/* Il tag <form> riceve gli stili globali da index.css */}
      <form onSubmit={handleAddLocation}>
        <div> {/* Il div per label+input riceve stili globali */}
          {/* Potresti aggiungere una <label> qui se vuoi */}
          <input
            type="text"
            value={newLocationName}
            onChange={(e) => setNewLocationName(e.target.value)}
            placeholder="Enter city name or ZIP"
            disabled={isAdding}
            // Gli stili di input sono ora globali
          />
        </div>
        <button type="submit" disabled={isAdding}>
          {isAdding ? 'Adding...' : 'Add Location'}
        </button>
      </form>

      {/* Applica la classe .error-message se c'è un errore */}
      {error && <p className="error-message">{error}</p>}

      <h3>Your Favorite Locations</h3>
      {favorites.length === 0 && !isLoading && (
        <p>You don't have any favorite locations yet. Add one above!</p>
      )}
      {/* Applica la classe contenitore per le card */}
      <div className="weather-card-container">
        {favorites.map((fav) => (
          // Applica la classe .card
          <div key={fav.id} className="card">
            <h4>{fav.name}</h4>
            <p>Lat: {fav.latitude.toFixed(2)}, Lon: {fav.longitude.toFixed(2)}</p>
            
            {fav.weatherData ? (
              <>
                {fav.weatherData.current ? (
                  // Applica la classe .weather-info-section
                  <div className="weather-info-section">
                    <strong>Current Weather:</strong>
                    <p>Temp: {fav.weatherData.current.temperature}°C (Feels like: {fav.weatherData.current.apparentTemperature}°C)</p>
                    <p>Condition: {fav.weatherData.current.weatherDescription} (Code: {fav.weatherData.current.weatherCode})</p>
                    <p>Humidity: {fav.weatherData.current.relativeHumidity}%</p>
                    <p>Wind: {fav.weatherData.current.windSpeed} km/h</p>
                  </div>
                ) : (
                  // Applica la classe .italic-message
                  <p className="italic-message">Current weather conditions are not available.</p>
                )}

                {fav.weatherData.daily && fav.weatherData.daily.length > 0 ? (
                  // Applica la classe .weather-info-section
                  <div className="weather-info-section">
                    <strong>Forecast (Next days):</strong>
                    {fav.weatherData.daily.slice(0, 3).map((day, index) => (
                      <div key={index} style={{ marginTop: '5px', paddingTop: '5px', borderTop: index > 0 ? '1px dashed #f0f0f0' : 'none'}}>
                        <p>
                          <strong>{new Date(day.date).toLocaleDateString('it-IT', { weekday: 'short', day: 'numeric', month: 'short' })}:</strong> 
                          Max {day.temperatureMax}°C, Min {day.temperatureMin}°C - {day.weatherDescription}
                        </p>
                      </div>
                    ))}
                  </div>
                ) : (
                  // Applica la classe .italic-message
                  <p className="italic-message">Daily forecast is not available.</p>
                )}
              </>
            ) : (
              // Applica la classe .italic-message
              <p className="italic-message">Weather data is completely unavailable for this location.</p>
            )}

            <button 
              onClick={() => handleDeleteLocation(fav.id)} 
              className="delete-button" // Applica la classe .delete-button
            >
              Delete
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}

export default DashboardPage;
