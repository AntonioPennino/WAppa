// src/pages/LoginPage.jsx
import React, { useState, useEffect } from 'react'; // Aggiunto useEffect
import { Link, useNavigate, useLocation } from 'react-router-dom'; // Aggiunto useLocation
import { loginUser } from '../services/authService';
import { useAuth } from '../contexts/AuthContext';

function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  
  const navigate = useNavigate();
  const location = useLocation(); // Per ottenere lo stato 'from'
  const { login: contextLogin, isAuthenticated } = useAuth(); // Aggiunto isAuthenticated

  const from = location.state?.from?.pathname || '/dashboard'; // Pagina di default a cui reindirizzare

  // Se l'utente è già loggato e arriva alla pagina di login, reindirizzalo
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
        console.log('Login API call successful. Token sent to AuthContext.');
        navigate(from, { replace: true }); // Reindirizza alla pagina 'from' o a /dashboard
      } else {
        setError(response.message || 'Login failed due to an unexpected issue.');
      }
    } catch (err) {
      console.error('Login error caught in LoginPage component:', err);
      setError(err.message || 'Login failed. Please check your credentials or network connection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    // ... JSX del form (rimane lo stesso)
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
