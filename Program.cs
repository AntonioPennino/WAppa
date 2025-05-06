// Program.cs
using Microsoft.EntityFrameworkCore; // Necessario per Entity Framework Core
using WeatherApp.Data;             // Namespace del tuo ApplicationDbContext (assicurati sia corretto)

var builder = WebApplication.CreateBuilder(args);

// --- Inizio Sezione Servizi ---

// 1. Aggiungi il supporto per i Controller API
// Questo è necessario se utilizzerai classi Controller per le tue API (es. AuthController, WeatherController)
builder.Services.AddControllers();

// 2. Configurazione di Entity Framework Core con SQLite
// Prende la stringa di connessione da appsettings.json o usa un default
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weatherapp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 3. Configurazione per Swagger/OpenAPI (già presente nel tuo codice)
// Utile per testare le API dal browser
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Fine Sezione Servizi ---


var app = builder.Build();

// --- Inizio Configurazione Pipeline HTTP ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANTE: Aggiungeremo queste righe più avanti quando implementeremo l'autenticazione JWT
// app.UseAuthentication(); // Prima UseAuthorization
// app.UseAuthorization();

// 4. Mappa le richieste ai Controller API
// Questa riga dice all'applicazione di usare i tuoi Controller per gestire le richieste HTTP
app.MapControllers();

// --- Fine Configurazione Pipeline HTTP ---

app.Run();