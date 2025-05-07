// src/services/favoriteLocationService.js
import axios from 'axios';

// L'URL base dell'API per le località preferite, letto dalla variabile d'ambiente.
// Esempio in .env.local: VITE_API_BASE_URL=https://localhost:7123/api
// Quindi FAVORITES_API_URL diventerà https://localhost:7123/api/favoritelocations
const FAVORITES_API_URL = `${import.meta.env.VITE_API_BASE_URL}/favoritelocations`;

/**
 * Recupera tutte le località preferite per l'utente autenticato.
 * Il token JWT viene inviato automaticamente negli header da Axios (configurato da setAuthHeader).
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: Array<FavoriteLocationResponseDto>, success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const getFavoriteLocations = async () => {
  try {
    const response = await axios.get(FAVORITES_API_URL);
    // Il backend restituisce ServiceResponse<List<FavoriteLocationResponseDto>>
    // La struttura è:
    // {
    //   data: [ { id, name, latitude, longitude, weatherData: { ... } }, ... ],
    //   success: true,
    //   message: "Favorite locations retrieved."
    // }
    return response.data;
  } catch (error) {
    console.error('Get Favorite Locations API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data; // Rilancia l'oggetto di errore del backend
    }
    throw new Error(error.message || 'An unknown error occurred while fetching favorite locations.');
  }
};

/**
 * Aggiunge una nuova località preferita per l'utente autenticato.
 * @param {string} locationName - Il nome della località da aggiungere (es. "Roma", "New York").
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: FavoriteLocationResponseDto, success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const addFavoriteLocation = async (locationName) => {
  try {
    // Il backend si aspetta un oggetto AddFavoriteLocationDto: { locationName: "string" }
    const payload = { locationName };
    const response = await axios.post(FAVORITES_API_URL, payload);
    // Il backend restituisce ServiceResponse<FavoriteLocationResponseDto>
    // {
    //   data: { id, name, latitude, longitude, weatherData: { ... } }, // la località aggiunta
    //   success: true,
    //   message: "Location '...' added to favorites."
    // }
    return response.data;
  } catch (error) {
    console.error('Add Favorite Location API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while adding favorite location.');
  }
};

/**
 * Rimuove una località preferita per l'utente autenticato.
 * @param {number} locationId - L'ID della località da rimuovere.
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: null (o stringa messaggio), success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const deleteFavoriteLocation = async (locationId) => {
  try {
    const response = await axios.delete(`${FAVORITES_API_URL}/${locationId}`);
    // Il backend restituisce ServiceResponse<string>
    // {
    //   data: "Favorite location '...' (ID: ...) deleted successfully.", // o potrebbe essere null
    //   success: true,
    //   message: "Favorite location '...' (ID: ...) deleted successfully."
    // }
    return response.data;
  } catch (error) {
    console.error('Delete Favorite Location API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while deleting favorite location.');
  }
};

/*
  Ricorda:
  - Assicurati che VITE_API_BASE_URL sia impostato correttamente nel tuo file .env.local.
  - Il token JWT per l'autenticazione viene aggiunto automaticamente alle richieste
    da Axios se setAuthHeader() è stato chiamato dopo il login (come abbiamo fatto in AuthContext).
  - Il backend (FavoriteLocationsController) deve essere protetto con [Authorize]
    e gestire correttamente l'ID utente dal token.
*/
