# WAppa/Frontend/WAppa-frontend/Dockerfile

# --- Fase 1: Build dell'applicazione React ---
# Utilizza un'immagine Node.js ufficiale come ambiente di build.
# Scegli una versione LTS stabile di Node.js (es. 20-alpine per una versione leggera).
FROM node:20-alpine AS build-stage
LABEL maintainer="tua_email@example.com"
# Opzionale: aggiungi il tuo nome/email

# Imposta la directory di lavoro all'interno del container.
WORKDIR /app

# Copia package.json e package-lock.json (o yarn.lock) per prima cosa.
# Questo sfrutta il caching di Docker: se questi file non cambiano,
# Docker può riutilizzare il layer delle dipendenze installate.
COPY package*.json ./

# Installa le dipendenze del progetto.
# Se usi npm ci, che è più veloce e sicuro per le build CI/CD: RUN npm ci
RUN npm install

# Copia tutto il resto del codice sorgente dell'applicazione nella directory /app.
COPY . .

# Definisci un argomento di build per l'URL base dell'API.
# Questo valore verrà passato da Docker Compose o dal comando 'docker build'.
# Fornisci un valore di default ragionevole per lo sviluppo locale se buildi l'immagine manualmente.
ARG VITE_API_BASE_URL_ARG=http://localhost:5000/api 
# NOTA: Il valore di default qui è solo un placeholder.
# Docker Compose sovrascriverà questo con http://backend:PORTA_BACKEND/api

# Imposta la variabile d'ambiente VITE_API_BASE_URL usando l'argomento di build.
# Vite userà questa variabile durante il processo di build.
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL_ARG

# Logga la variabile d'ambiente per debug durante la build
RUN echo "Building frontend with VITE_API_BASE_URL: $VITE_API_BASE_URL"

# Esegui lo script di build per produrre i file statici ottimizzati.
# L'output della build di Vite di solito va nella cartella 'dist'.
RUN npm run build

# --- Fase 2: Servire l'applicazione con Nginx ---
# Utilizza un'immagine Nginx ufficiale e leggera.
FROM nginx:stable-alpine

# Rimuovi la configurazione di default di Nginx.
RUN rm /etc/nginx/conf.d/default.conf

# Copia la tua configurazione Nginx personalizzata nel container.
# Creeremo questo file 'nginx.conf' separatamente.
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copia i file statici buildati dalla fase 'build-stage' (dalla cartella /app/dist)
# nella directory web root di Nginx.
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Esponi la porta 80, che è la porta standard su cui Nginx ascolta.
EXPOSE 80

# Il comando di default per l'immagine nginx è già 'nginx -g "daemon off;"',
# quindi non è necessario specificare un CMD a meno che non si voglia cambiarlo.
# CMD ["nginx", "-g", "daemon off;"]
