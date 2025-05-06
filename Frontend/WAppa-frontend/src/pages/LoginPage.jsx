// src/pages/LoginPage.jsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom'; // useNavigate per il redirect dopo il login
// Importeremo il servizio di autenticazione più avanti
// import { loginUser } from '../services/authService'; 

function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);

    if (!username || !password) {
      setError('Username and password are required.');
      setLoading(false);
      return;
    }

    console.log('Login attempt with:', { username, password });
    // Qui chiameremo il servizio di autenticazione
    // try {
    //   const response = await loginUser({ username, password });
    //   console.log('Login successful:', response);
    //   // Salva il token, aggiorna lo stato dell'utente (useremo Context API)
    //   // Esempio: authContext.login(response.data.token, response.data.username);
    //   navigate('/dashboard'); // Reindirizza al dashboard
    // } catch (err) {
    //   setError(err.response?.data?.message || 'Login failed. Please try again.');
    // } finally {
    //   setLoading(false);
    // }
    
    // Simulazione per ora
    setTimeout(() => {
      if (username === "test" && password === "password") {
        console.log("Simulated login successful");
        navigate('/dashboard');
      } else {
        setError('Invalid credentials (simulated).');
      }
      setLoading(false);
    }, 1000);
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
