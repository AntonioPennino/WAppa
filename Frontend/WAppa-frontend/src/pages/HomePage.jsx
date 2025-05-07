// src/pages/HomePage.jsx
import React, { useState } from 'react';
import { searchWeatherByQuery } from '../services/weatherService';

// Non abbiamo più bisogno degli stili inline definiti qui
// const cardStyle = { ... };
// const weatherInfoStyle = { ... };
// const italicMessageStyle = { ... };

function HomePage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [weatherResult, setWeatherResult] = useState(null);
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
    setWeatherResult(null);

    try {
      const response = await searchWeatherByQuery(searchQuery);
      if (response.success && response.data) {
        console.log("Weather search result on HomePage:", response.data);
        setWeatherResult(response.data);
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
    <div className="homepage-container"> {/* Aggiunta classe contenitore opzionale */}
      <h2>Welcome to the Weather App!</h2>
      <p>Your one-stop solution for weather forecasts.</p>

      {/* Il tag <form> riceve gli stili globali da index.css */}
      {/* Puoi aggiungere una classe specifica se vuoi personalizzare ulteriormente questo form */}
      <form onSubmit={handleSearch} className="weather-search-form"> 
        <div> {/* Rimosso div superfluo, gli stili di input e button sono globali */}
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Enter city name or ZIP"
              disabled={isLoading}
              // Gli stili sono ora globali, puoi aggiungere classi per override se necessario
            />
        </div>
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Searching...' : 'Search Weather'}
        </button>
      </form>

      {/* Applica la classe .error-message se c'è un errore */}
      {error && <p className="error-message">{error}</p>}

      {isLoading && <div className="loading-message" style={{ marginTop: '20px' }}>Loading weather data...</div>}

      {weatherResult && (
        // Applica la classe .card
        // Potremmo aggiungere una classe per il contenitore della card risultato se necessario
        <div className="card weather-result-card"> 
          <h3>Weather for {weatherResult.locationName || searchQuery}</h3> 
          <p>Lat: {weatherResult.latitude.toFixed(2)}, Lon: {weatherResult.longitude.toFixed(2)}</p>

          {weatherResult.current ? (
            // Applica la classe .weather-info-section
            <div className="weather-info-section">
              <strong>Current Weather:</strong>
              <p>Temp: {weatherResult.current.temperature}°C (Feels like: {weatherResult.current.apparentTemperature}°C)</p>
              <p>Condition: {weatherResult.current.weatherDescription} (Code: {weatherResult.current.weatherCode})</p>
              <p>Humidity: {weatherResult.current.relativeHumidity}%</p>
              <p>Wind: {weatherResult.current.windSpeed} km/h</p>
            </div>
          ) : (
            // Applica la classe .italic-message
            <p className="italic-message">Current weather conditions are not available.</p>
          )}

          {weatherResult.daily && weatherResult.daily.length > 0 ? (
            // Applica la classe .weather-info-section
            <div className="weather-info-section">
              <strong>Forecast:</strong>
              {weatherResult.daily.slice(0, 3).map((day, index) => (
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
        </div>
      )}
    </div>
  );
}

export default HomePage;
