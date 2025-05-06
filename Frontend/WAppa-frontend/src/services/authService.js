// src/services/authService.js
import axios from 'axios';

// L'URL base dell'API di autenticazione, letto dalla variabile d'ambiente.
// Verrà sostituito in fase di build da Vite con il valore definito in .env.local
// Esempio in .env.local: VITE_API_BASE_URL=https://localhost:7123/api
// Quindi AUTH_API_URL diventerà https://localhost:7123/api/auth
const AUTH_API_URL = `${import.meta.env.VITE_API_BASE_URL}/auth`;

/**
 * Effettua il login dell'utente.
 * @param {object} credentials - Oggetto con username e password.
 * @param {string} credentials.username - Username dell'utente.
 * @param {string} credentials.password - Password dell'utente.
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: { token: "...", username: "..." }, success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const loginUser = async (credentials) => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/login`, credentials);
    // Il tuo backend restituisce una ServiceResponse<AuthResponseDto>
    // La struttura è:
    // {
    //   data: { token: "...", username: "..." }, // Questo è AuthResponseDto
    //   success: true,
    //   message: "Login successful!"
    // }
    // Restituiamo l'intero oggetto response.data così il componente può accedere a 'success', 'message' e 'data'.
    return response.data;
  } catch (error) {
    // Gestione degli errori di Axios
    // error.response contiene la risposta dal server se la richiesta è stata fatta e il server ha risposto con un codice di stato
    // che cade al di fuori dell'intervallo di 2xx.
    // error.request contiene la richiesta se è stata fatta ma non è stata ricevuta alcuna risposta.
    // error.message è il messaggio di errore.
    console.error('Login API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      // Se il backend restituisce un corpo di errore strutturato (es. la nostra ServiceResponse con Success=false)
      throw error.response.data; // Rilancia l'oggetto di errore del backend
    }
    throw new Error(error.message || 'An unknown error occurred during login.');
  }
};

/**
 * Registra un nuovo utente.
 * @param {object} userData - Oggetto con username e password.
 * @param {string} userData.username - Username per il nuovo utente.
 * @param {string} userData.password - Password per il nuovo utente.
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: userId (int), success: true, message: "..." }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 */
export const registerUser = async (userData) => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/register`, userData);
    // Il backend restituisce una ServiceResponse<int>
    // La struttura è:
    // {
    //   data: 123, // ID del nuovo utente
    //   success: true,
    //   message: "User registered successfully!"
    // }
    return response.data;
  } catch (error) {
    console.error('Register API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred during registration.');
  }
};

// --- Funzioni di utilità per il token JWT ---

const TOKEN_KEY = 'weatherAppToken'; // Chiave per localStorage

/**
 * Salva il token JWT nel localStorage.
 * @param {string} token - Il token JWT da salvare.
 */
export const saveToken = (token) => {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY); // Rimuovi se il token è nullo/undefined
  }
};

/**
 * Recupera il token JWT dal localStorage.
 * @returns {string|null} Il token JWT o null se non trovato.
 */
export const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Rimuove il token JWT dal localStorage (per il logout).
 */
export const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
};

/**
 * Configura l'header Authorization di default per Axios se un token è presente.
 * Questa funzione dovrebbe essere chiamata all'avvio dell'app o dopo il login.
 */
export const setAuthHeader = () => {
  const token = getToken();
  if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete axios.defaults.headers.common['Authorization'];
  }
};

// Potresti voler chiamare setAuthHeader() una volta all'avvio dell'app
// per configurare Axios se un token è già presente da una sessione precedente.
// Ad esempio, in main.jsx o App.jsx.
// setAuthHeader(); // Chiamata iniziale

/*
  IMPORTANTE: Configurazione CORS nel Backend .NET
  Perché queste chiamate API da React (es. http://localhost:5173) al tuo backend .NET (es. https://localhost:7123)
  funzionino, devi abilitare CORS (Cross-Origin Resource Sharing) nel tuo `Program.cs` del backend.
  Altrimenti, il browser bloccherà le richieste per motivi di sicurezza.

  Esempio di configurazione CORS base in Program.cs (backend .NET):

  var builder = WebApplication.CreateBuilder(args);

  // ... altri servizi

  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowReactApp",
          builder =>
          {
              builder.WithOrigins("http://localhost:5173") // L'URL del tuo frontend React
                     .AllowAnyHeader()
                     .AllowAnyMethod();
          });
  });

  var app = builder.Build();

  // ... altre configurazioni della pipeline

  app.UseHttpsRedirection();

  app.UseCors("AllowReactApp"); // IMPORTANTE: Inserisci UseCors prima di UseAuthentication e UseAuthorization

  app.UseAuthentication();
  app.UseAuthorization();

  app.MapControllers();
  app.Run();

  Assicurati che "http://localhost:5173" corrisponda all'URL su cui Vite esegue il tuo frontend.
  Per produzione, dovrai configurare CORS con l'URL effettivo del tuo frontend deployato.
*/
