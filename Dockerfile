# WAppa/Dockerfile (o nella cartella del tuo progetto backend principale)

# Fase 1: Build dell'applicazione
# Utilizza l'immagine SDK di .NET per compilare l'applicazione.
# Assicurati che il tag :8.0 corrisponda alla versione del tuo .NET TargetFramework.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia prima i file di progetto (.csproj) e ripristina le dipendenze.
# Questo passaggio sfrutta il layer caching di Docker: se i file .csproj non cambiano,
# Docker riutilizzerà il layer memorizzato nella cache per 'dotnet restore', velocizzando le build successive.
COPY *.csproj ./
RUN dotnet restore

# Copia tutto il resto del codice sorgente dell'applicazione nella directory /app del container.
COPY . ./

# Pubblica l'applicazione. 
# -c Release specifica la configurazione di build Release (ottimizzata).
# -o out specifica la cartella di output 'out' per i file pubblicati.
RUN dotnet publish -c Release -o out

# Fase 2: Creazione dell'immagine runtime
# Utilizza l'immagine runtime di ASP.NET Core, che è più leggera dell'SDK
# perché contiene solo il necessario per eseguire l'applicazione, non per compilarla.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env
WORKDIR /app

# Copia l'output della build (dalla cartella 'out' della fase 'build-env')
# nella directory /app dell'immagine runtime.
COPY --from=build-env /app/out .

# Esponi la porta su cui l'applicazione ASP.NET Core ascolterà ALL'INTERNO del container.
# Questo DEVE corrispondere alla porta che Kestrel è configurato per usare.
# Se il tuo ASPNETCORE_URLS nel docker-compose.yml sarà 'http://+:5278',
# allora Kestrel ascolterà sulla porta 5278 dentro il container.
EXPOSE 5278
# Se volessi HTTPS (e avessi gestito i certificati nel container), esporresti la porta HTTPS.
# Per ora, ci concentriamo su HTTP per la comunicazione interna tra container.

# Definisce il comando da eseguire quando il container viene avviato.
# Sostituisci 'WAppa.dll' con il nome effettivo del file DLL principale del tuo progetto backend.
# Questo file DLL si troverà nella root della cartella copiata da /app/out.
ENTRYPOINT ["dotnet", "WAppa.dll"]
