// src/pages/RegisterPage.jsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { registerUser } from '../services/authService'; // Importa la funzione

function RegisterPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setSuccessMessage('');
    setLoading(true);

    if (!username || !password || !confirmPassword) {
      setError('All fields are required.');
      setLoading(false);
      return;
    }

    if (password.length < 6) { // Aggiungiamo un controllo base come nel backend DTO
        setError('Password must be at least 6 characters long.');
        setLoading(false);
        return;
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      setLoading(false);
      return;
    }
    
    try {
      const response = await registerUser({ username, password });
      // response è l'oggetto { data: userId, success, message }

      if (response.success) {
        console.log('Registration successful. User ID:', response.data);
        setSuccessMessage(response.message + ' You can now log in.');
        // Opzionale: svuota i campi dopo la registrazione
        setUsername('');
        setPassword('');
        setConfirmPassword('');
        // Considera di reindirizzare al login dopo un breve ritardo o su click
        setTimeout(() => {
            navigate('/login');
        }, 3000); // Reindirizza dopo 3 secondi
      } else {
        setError(response.message || 'Registration failed due to an unexpected issue.');
      }
    } catch (err) {
      console.error('Registration error caught in component:', err);
      setError(err.message || 'Registration failed. Please try again or check network connection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Register</h2>
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
          <label htmlFor="password">Password (min 6 characters):</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        <div>
          <label htmlFor="confirmPassword">Confirm Password:</label>
          <input
            type="password"
            id="confirmPassword"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
        {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
        <button type="submit" disabled={loading}>
          {loading ? 'Registering...' : 'Register'}
        </button>
      </form>
      <p>
        Already have an account? <Link to="/login">Login here</Link>
      </p>
    </div>
  );
}

export default RegisterPage;
