// src/pages/HomePage.jsx
import React, { useState } from 'react';
import { searchWeatherByQuery } from '../services/weatherService'; // Importa la funzione

// Stili (simili a DashboardPage, potresti centralizzarli)
const cardStyle = {
  border: '1px solid #ddd',
  borderRadius: '8px',
  padding: '15px',
  margin: '20px auto', // Centrato
  width: '350px',
  boxShadow: '2px 2px 5px rgba(0,0,0,0.1)',
  textAlign: 'left'
};

const weatherInfoStyle = {
  marginTop: '10px',
  paddingTop: '10px',
  borderTop: '1px solid #eee'
};

const italicMessageStyle = {
  ...weatherInfoStyle, 
  fontStyle: 'italic',
  color: '#555'
};

function HomePage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [weatherResult, setWeatherResult] = useState(null); // Per i dati meteo trovati
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (event) => {
    event.preventDefault();
    if (!searchQuery.trim()) {
      setError('Please enter a city name or ZIP code to search.');
      setWeatherResult(null);
      return;
    }
    setIsLoading(true);
    setError('');
    setWeatherResult(null); // Resetta risultati precedenti

    try {
      const response = await searchWeatherByQuery(searchQuery);
      if (response.success && response.data) {
        console.log("Weather search result:", response.data); // LOG PER VERIFICARE locationName
        setWeatherResult(response.data); // response.data è WeatherForecastDto
      } else {
        setError(response.message || 'Could not find weather for the specified location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred during the weather search.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <h2>Welcome to the Weather App!</h2>
      <p>Your one-stop solution for weather forecasts.</p>

      <form onSubmit={handleSearch} style={{ margin: '20px 0' }}>
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Enter city name or ZIP"
          disabled={isLoading} // Aggiunto disabled qui
          style={{ marginRight: '10px', padding: '10px', minWidth: '250px', fontSize: '1em' }}
        />
        <button type="submit" disabled={isLoading} style={{ padding: '10px 18px', fontSize: '1em' }}>
          {isLoading ? 'Searching...' : 'Search Weather'}
        </button>
      </form>

      {error && <p style={{ color: 'red', marginTop: '10px' }}>{error}</p>}

      {isLoading && <div style={{ marginTop: '20px' }}>Loading weather data...</div>}

      {weatherResult && (
        <div style={cardStyle}>
          {/* MODIFICATO QUI per usare weatherResult.locationName */}
          <h3>Weather for {weatherResult.locationName || searchQuery}</h3> 
          <p>Lat: {weatherResult.latitude.toFixed(2)}, Lon: {weatherResult.longitude.toFixed(2)}</p>

          {/* Logica di rendering meteo (simile a DashboardPage) */}
          {weatherResult.current ? (
            <div style={weatherInfoStyle}>
              <strong>Current Weather:</strong>
              <p>Temp: {weatherResult.current.temperature}°C (Feels like: {weatherResult.current.apparentTemperature}°C)</p>
              <p>Condition: {weatherResult.current.weatherDescription} (Code: {weatherResult.current.weatherCode})</p>
              <p>Humidity: {weatherResult.current.relativeHumidity}%</p>
              <p>Wind: {weatherResult.current.windSpeed} km/h</p>
            </div>
          ) : (
            <p style={italicMessageStyle}>Current weather conditions are not available.</p>
          )}

          {weatherResult.daily && weatherResult.daily.length > 0 ? (
            <div style={weatherInfoStyle}>
              <strong>Forecast:</strong>
              {weatherResult.daily.slice(0, 3).map((day, index) => ( // Mostra i primi 3 giorni
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
        </div>
      )}
    </div>
  );
}

export default HomePage;
