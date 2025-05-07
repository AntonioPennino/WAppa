// src/contexts/AuthContext.jsx
import React, { createContext, useContext, useState, useEffect } from 'react';
import { getToken, saveToken, removeToken, setAuthHeader } from '../services/authService';
import { jwtDecode } from 'jwt-decode';

// 1. Creare il Context
const AuthContext = createContext(null);

// 2. Creare il Provider Component
export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(null);
  const [token, setTokenState] = useState(null); // Inizializza a null, poi useEffect legge da localStorage
  const [loadingAuth, setLoadingAuth] = useState(true); // Stato di caricamento specifico per l'autenticazione

  // Funzione interna per resettare lo stato di autenticazione
  const resetAuthState = () => {
    removeToken();
    setTokenState(null);
    setCurrentUser(null);
    setAuthHeader(); // Rimuove l'header di default
  };

  useEffect(() => {
    let isMounted = true; // Flag per gestire cleanup ed evitare setState su componente smontato

    const initializeAuth = () => {
      console.log("AuthContext: Initializing authentication state...");
      const existingToken = getToken();

      if (existingToken) {
        try {
          const decodedToken = jwtDecode(existingToken);
          const currentTime = Date.now() / 1000; // in secondi

          if (decodedToken.exp < currentTime) {
            console.log("AuthContext useEffect: Token scaduto, rimozione...");
            if (isMounted) {
              resetAuthState();
            }
          } else {
            // VERIFICA QUI IL CLAIM CORRETTO PER LO USERNAME (es. decodedToken.name, decodedToken.unique_name)
            const usernameFromToken = decodedToken.unique_name || decodedToken.name; // Prova unique_name, poi name
            const userIdFromToken = decodedToken.nameid;

            if (!usernameFromToken) {
                console.warn("AuthContext useEffect: Username claim (unique_name or name) non trovato nel token:", decodedToken);
            }
            if (!userIdFromToken) {
                console.warn("AuthContext useEffect: User ID claim (nameid) non trovato nel token:", decodedToken);
            }

            console.log(`AuthContext useEffect: Token valido trovato. Username da claim ('${usernameFromToken ? 'unique_name/name' : 'NON TROVATO'}'): ${usernameFromToken}, ID: ${userIdFromToken}`);
            
            if (isMounted) {
              setCurrentUser({ 
                username: usernameFromToken, 
                id: userIdFromToken 
              });
              setTokenState(existingToken);
              setAuthHeader(); // Assicura che Axios abbia l'header se il token è valido
            }
          }
        } catch (error) {
          console.error("AuthContext useEffect: Errore nel decodificare il token o token invalido:", error);
          if (isMounted) {
            resetAuthState();
          }
        }
      } else {
        console.log("AuthContext useEffect: Nessun token esistente trovato.");
        // Non c'è token, quindi non c'è utente. Lo stato è già null di default.
        // setAuthHeader() verrà chiamato da resetAuthState se necessario, o è già senza token.
      }

      // setLoadingAuth(false) viene chiamato solo dopo che tutto è stato processato
      if (isMounted) {
        setLoadingAuth(false);
        console.log("AuthContext: Authentication initialization complete. loadingAuth set to false.");
      }
    };

    initializeAuth();

    return () => {
      isMounted = false; // Funzione di cleanup per l'effetto
    };
  }, []); // Array di dipendenze vuoto per eseguire solo una volta al mount

  const login = (jwtToken) => {
    console.log("AuthContext: login chiamato con token");
    saveToken(jwtToken); // Salva prima il token
    setAuthHeader();    // Imposta l'header subito dopo averlo salvato
    try {
      const decodedToken = jwtDecode(jwtToken);
      // VERIFICA QUI IL CLAIM CORRETTO PER LO USERNAME
      const usernameFromToken = decodedToken.unique_name || decodedToken.name;
      const userIdFromToken = decodedToken.nameid;

      if (!usernameFromToken) {
          console.warn("AuthContext login: Username claim (unique_name or name) non trovato nel token:", decodedToken);
      }
      if (!userIdFromToken) {
          console.warn("AuthContext login: User ID claim (nameid) non trovato nel token:", decodedToken);
      }

      console.log(`AuthContext login: Decoded token. Username da claim: ${usernameFromToken}, ID: ${userIdFromToken}`);
      setCurrentUser({ 
        username: usernameFromToken,
        id: userIdFromToken
      });
      setTokenState(jwtToken); // Aggiorna lo stato del token qui
    } catch (error) {
      console.error("AuthContext login: Errore nel decodificare il token:", error);
      resetAuthState(); // In caso di errore nella decodifica del nuovo token, resetta tutto
    }
  };

  const logout = () => {
    console.log("AuthContext: logout chiamato");
    resetAuthState();
  };

  const isAuthenticated = () => {
    // Per essere autenticato, deve esserci un token e un utente (con username)
    return !!token && !!currentUser && !!currentUser.username;
  };

  const value = {
    currentUser,
    token,
    isAuthenticated,
    login,
    logout,
    loadingAuth // Usa la variabile di stato rinominata
  };

  if (loadingAuth) { // Usa la variabile di stato rinominata
    return <div>Loading authentication status...</div>; // O uno spinner, o null
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
