// src/pages/LoginPage.jsx
import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { loginUser } from '../services/authService';
import { useAuth } from '../contexts/AuthContext';

// Opzionale: importare CSS Modules se vuoi stili ancora più specifici per questa pagina
// import styles from './LoginPage.module.css'; 

function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  
  const navigate = useNavigate();
  const location = useLocation();
  const { login: contextLogin, isAuthenticated } = useAuth();

  const from = location.state?.from?.pathname || '/dashboard';

  useEffect(() => {
    if (isAuthenticated()) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

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
      if (response.success && response.data?.token) {
        contextLogin(response.data.token);
        navigate(from, { replace: true });
      } else {
        setError(response.message || 'Login failed due to an unexpected issue.');
      }
    } catch (err) {
      setError(err.message || 'Login failed. Please check your credentials or network connection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    // Puoi aggiungere una classe contenitore se usi CSS Modules, es: <div className={styles.loginContainer}>
    <div> 
      <h2>Login</h2>
      {/* Il tag <form> ora riceverà gli stili da index.css */}
      <form onSubmit={handleSubmit}>
        <div> {/* Questo div raggruppa label e input e riceve stili da index.css */}
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            disabled={loading}
            required
            autoComplete="username"
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
            autoComplete="current-password"
          />
        </div>
        {/* Applica la classe .error-message */}
        {error && <p className="error-message">{error}</p>} 
        <button type="submit" disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      <p style={{ marginTop: '20px' }}> {/* Aggiunto un po' di spazio sopra questo paragrafo */}
        Don't have an account? <Link to="/register" style={{ color: '#3498db', fontWeight: 'bold' }}>Register here</Link>
      </p>
    </div>
  );
}

export default LoginPage;
