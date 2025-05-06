// src/pages/LoginPage.jsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { loginUser, saveToken, setAuthHeader } from '../services/authService'; // Importa le funzioni necessarie

// In futuro, importeremo AuthContext per aggiornare lo stato globale
// import { useAuth } from '../contexts/AuthContext';

function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  // const { login } = useAuth(); // Lo useremo con AuthContext

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);

    if (!username || !password) {
      setError('Username and password are required.');
      setLoading(false);
      return;
    }

    try {
      const response = await loginUser({ username, password });
      // response è l'oggetto { data: { token, username }, success, message }
      
      if (response.success && response.data?.token) {
        console.log('Login successful:', response.data.username);
        saveToken(response.data.token); // Salva il token in localStorage
        setAuthHeader(); // Imposta l'header Authorization di default per Axios
        
        // Aggiorna lo stato di autenticazione globale (lo faremo con AuthContext)
        // login(response.data.username, response.data.token); 

        navigate('/dashboard'); // Reindirizza al dashboard
      } else {
        // Se success è false o non c'è token, usa il messaggio dal backend
        setError(response.message || 'Login failed due to an unexpected issue.');
      }
    } catch (err) {
      // err qui dovrebbe essere l'oggetto ServiceResponse con Success=false
      // o un errore generico se la chiamata API è fallita a un livello inferiore.
      console.error('Login error caught in component:', err);
      setError(err.message || 'Login failed. Please check your credentials or network connection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        <div>
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit" disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      <p>
        Don't have an account? <Link to="/register">Register here</Link>
      </p>
    </div>
  );
}

export default LoginPage;
