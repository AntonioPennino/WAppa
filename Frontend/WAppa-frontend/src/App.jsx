// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import './App.css';

// Importa i nuovi componenti pagina
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import NotFoundPage from './pages/NotFoundPage';

function App() {
  // In futuro, qui potremmo avere lo stato dell'utente per mostrare/nascondere link
  // const isAuthenticated = false; // Esempio

  return (
    <Router>
      <div className="App">
        <nav style={{ marginBottom: '20px', borderBottom: '1px solid #ccc', paddingBottom: '10px' }}>
          <ul style={{ listStyleType: 'none', padding: 0, margin: 0, display: 'flex', justifyContent: 'center', gap: '15px' }}>
            <li><Link to="/">Home</Link></li>
            {/* Logica condizionale per i link di navigazione in base all'autenticazione */}
            {/* {isAuthenticated ? (
              <>
                <li><Link to="/dashboard">Dashboard</Link></li>
                <li><button onClick={handleLogout}>Logout</button></li> 
              </>
            ) : (
              <> */}
                <li><Link to="/login">Login</Link></li>
                <li><Link to="/register">Register</Link></li>
              {/* </>
            )} */}
             <li><Link to="/dashboard">Dashboard (Accesso temporaneo per test)</Link></li> {/* Rimuovere/proteggere in seguito */}
          </ul>
        </nav>

        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          {/* La route /dashboard dovrà essere protetta in futuro */}
          <Route path="/dashboard" element={<DashboardPage />} /> 
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
