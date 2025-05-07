// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Link, useNavigate } from 'react-router-dom';
import './App.css'; // Questo file potrebbe essere vuoto o avere stili specifici per App.jsx
import { useAuth } from './contexts/AuthContext';

// Importa le pagine e i componenti
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import NotFoundPage from './pages/NotFoundPage';
import ProtectedRoute from './components/ProtectedRoute';

function Navigation() {
  const { isAuthenticated, logout, currentUser } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav> {/* Rimossi stili inline, ora gestiti da index.css */}
      <ul> {/* Rimossi stili inline */}
        <li><Link to="/">Home</Link></li>
        {isAuthenticated() ? (
          <>
            <li><Link to="/dashboard">Dashboard</Link></li>
            <li>
              {/* Aggiunta classe nav-button per il bottone di logout */}
              <button onClick={handleLogout} className="nav-button">
                Logout ({currentUser?.username || 'User'})
              </button>
            </li>
          </>
        ) : (
          <>
            <li><Link to="/login">Login</Link></li>
            <li><Link to="/register">Register</Link></li>
          </>
        )}
      </ul>
    </nav>
  );
}

function App() {
  const { loadingAuth } = useAuth();

  if (loadingAuth) {
    return <div className="loading-app">Loading application...</div>; // Aggiunta classe
  }

  return (
    <Router>
      <div className="App"> {/* Questa classe è già definita in index.css */}
        <Navigation />
        
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            } 
          />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
