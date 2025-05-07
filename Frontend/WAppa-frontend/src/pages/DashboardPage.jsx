// src/pages/DashboardPage.jsx
import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { 
  getFavoriteLocations, 
  addFavoriteLocation, 
  deleteFavoriteLocation 
} from '../services/favoriteLocationService';

// Stile di base per le card (puoi spostarlo in un file CSS)
const cardStyle = {
  border: '1px solid #ddd',
  borderRadius: '8px',
  padding: '15px',
  margin: '10px',
  width: '300px',
  boxShadow: '2px 2px 5px rgba(0,0,0,0.1)',
  textAlign: 'left'
};

const weatherInfoStyle = {
  marginTop: '10px',
  paddingTop: '10px',
  borderTop: '1px solid #eee'
};

const italicMessageStyle = { // Stile per i messaggi di "non disponibilità"
  ...weatherInfoStyle, 
  fontStyle: 'italic',
  color: '#555' // Un colore leggermente più tenue
};

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
    return <div>Loading your favorite locations...</div>;
  }

  return (
    <div>
      <h2>My Dashboard</h2>
      {currentUser && <p>Welcome back, {currentUser.username}!</p>}

      <h3>Add New Favorite Location</h3>
      <form onSubmit={handleAddLocation} style={{ marginBottom: '20px' }}>
        <input
          type="text"
          value={newLocationName}
          onChange={(e) => setNewLocationName(e.target.value)}
          placeholder="Enter city name or ZIP"
          disabled={isAdding}
          style={{ marginRight: '10px', padding: '8px', minWidth: '200px' }}
        />
        <button type="submit" disabled={isAdding} style={{ padding: '8px 15px' }}>
          {isAdding ? 'Adding...' : 'Add Location'}
        </button>
      </form>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <h3>Your Favorite Locations</h3>
      {favorites.length === 0 && !isLoading && (
        <p>You don't have any favorite locations yet. Add one above!</p>
      )}
      <div style={{ display: 'flex', flexWrap: 'wrap', justifyContent: 'center' }}>
        {favorites.map((fav) => (
          <div key={fav.id} style={cardStyle}>
            <h4>{fav.name}</h4>
            <p>Lat: {fav.latitude.toFixed(2)}, Lon: {fav.longitude.toFixed(2)}</p>
            
            {/* Logica di rendering meteo aggiornata */}
            {fav.weatherData ? (
              <> {/* Contenitore React Fragment per i dati meteo */}
                {fav.weatherData.current ? (
                  <div style={weatherInfoStyle}>
                    <strong>Current Weather:</strong>
                    <p>Temp: {fav.weatherData.current.temperature}°C (Feels like: {fav.weatherData.current.apparentTemperature}°C)</p>
                    <p>Condition: {fav.weatherData.current.weatherDescription} (Code: {fav.weatherData.current.weatherCode})</p>
                    <p>Humidity: {fav.weatherData.current.relativeHumidity}%</p>
                    <p>Wind: {fav.weatherData.current.windSpeed} km/h</p>
                  </div>
                ) : (
                  <p style={italicMessageStyle}>Current weather conditions are not available.</p>
                )}

{fav.weatherData && fav.weatherData.daily && fav.weatherData.daily.length > 0 ? (
              <div style={weatherInfoStyle}>
                <strong>Forecast (Next days):</strong> {/* Titolo aggiornato */}
                {fav.weatherData.daily.slice(0, 3).map((day, index) => ( // MOSTRA I PRIMI 3 GIORNI
                  <div key={index} style={{ marginTop: '5px', paddingTop: '5px', borderTop: index > 0 ? '1px dashed #f0f0f0' : 'none'}}>
                    <p>
                      <strong>{new Date(day.date).toLocaleDateString('it-IT', { weekday: 'short', day: 'numeric', month: 'short' })}:</strong> 
                      Max {day.temperatureMax}°C, Min {day.temperatureMin}°C - {day.weatherDescription}
                    </p>
                  </div>
                ))}
              </div>
            ) : (
              <p style={italicMessageStyle}>Daily forecast is not available.</p>
            )}
              </>
            ) : (
              <p style={italicMessageStyle}>Weather data is completely unavailable for this location.</p>
            )}
            {/* Fine logica di rendering meteo aggiornata */}

            <button 
              onClick={() => handleDeleteLocation(fav.id)} 
              style={{ marginTop: '10px', backgroundColor: '#ff4d4d', color: 'white', border: 'none', padding: '5px 10px', borderRadius: '4px', cursor: 'pointer' }}
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
