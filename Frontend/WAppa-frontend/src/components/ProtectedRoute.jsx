// src/components/ProtectedRoute.jsx
import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loadingAuth } = useAuth();
  const location = useLocation(); // Per ricordare da dove l'utente proveniva

  if (loadingAuth) {
    // Mostra un caricamento mentre lo stato di autenticazione viene verificato
    // Questo previene un redirect prematuro al login se il token è ancora in fase di verifica.
    return <div>Loading authentication...</div>; // O uno spinner
  }

  if (!isAuthenticated()) {
    // L'utente non è autenticato, reindirizzalo alla pagina di login.
    // Passiamo la posizione corrente (`location.pathname`) come stato,
    // così la pagina di login può reindirizzare l'utente indietro dopo un login riuscito.
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // L'utente è autenticato.
  // Se `children` è fornito, renderizza i figli (utile per layout specifici protetti).
  // Altrimenti, renderizza `<Outlet />` che renderizzerà il componente della route nidificata.
  // Per il nostro caso d'uso più comune con <Route element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
  // o <Route path="/dashboard" element={<ProtectedRoute><DashboardPage/></ProtectedRoute>} />
  // la logica qui sotto con Outlet è più flessibile per le route definite in App.jsx
  // Se volessimo passare il componente direttamente come children, non avremmo bisogno di Outlet.
  // return children ? children : <Outlet />;

  // Per la configurazione tipica dove <ProtectedRoute /> avvolge l'elemento della route:
  // <Route path="/dashboard" element={<ProtectedRoute element={<DashboardPage />} />} />
  // o meglio ancora, annidando le route, si usa <Outlet /> per renderizzare il componente figlio
  // definito nella route.
  // Per ora, usiamo una struttura semplice: se sei autenticato, ti faccio passare.
  // Il componente vero e proprio (es. DashboardPage) sarà l' `element` della Route.
  // Quindi questo componente farà da wrapper:
  return children; // Se la route è <Route path="/dashboard" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
                   // Allora DashboardPage è `children` qui.
};

export default ProtectedRoute;
