// src/services/weatherService.js
import axios from 'axios';

// L'URL base dell'API per il controller Weather, letto dalla variabile d'ambiente.
// Esempio in .env.local: VITE_API_BASE_URL=https://localhost:7123/api
// Quindi WEATHER_API_URL diventerà https://localhost:7123/api/weather
const WEATHER_API_URL = `${import.meta.env.VITE_API_BASE_URL}/weather`;

/**
 * Cerca i dati meteo per una località specificata da una query (nome città, CAP).
 * @param {string} query - La stringa di ricerca per la località.
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: WeatherForecastDto, success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const searchWeatherByQuery = async (query) => {
  try {
    // La richiesta GET usa parametri di query: /api/weather?query=NomeCitta
    const response = await axios.get(WEATHER_API_URL, {
      params: { query } 
    });
    // Il backend restituisce ServiceResponse<WeatherForecastDto>
    return response.data;
  } catch (error) {
    console.error('Search Weather by Query API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data; // Rilancia l'oggetto di errore del backend
    }
    throw new Error(error.message || 'An unknown error occurred while searching weather by query.');
  }
};

/**
 * Recupera i dati meteo per coordinate geografiche specifiche.
 * @param {number} latitude - La latitudine.
 * @param {number} longitude - La longitudine.
 * @returns {Promise<object>} La risposta dal backend.
 * @throws {Error} Se la chiamata API fallisce.
 */
export const getWeatherByCoordinates = async (latitude, longitude) => {
  try {
    const response = await axios.get(WEATHER_API_URL, {
      params: { latitude, longitude }
    });
    return response.data;
  } catch (error) {
    console.error('Get Weather by Coordinates API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while fetching weather by coordinates.');
  }
};

/*
  Ricorda:
  - Assicurati che VITE_API_BASE_URL sia impostato correttamente nel tuo file .env.local.
  - Queste chiamate sono pubbliche (non richiedono token JWT) perché il WeatherController
    nel backend non è protetto da [Authorize].
*/
