// Program.cs
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
// using WeatherApp.Services; // La direttiva using per 'WeatherApp.Services' è già presente più avanti
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using WeatherApp.Services; // Qui è presente, quindi quella sopra era duplicata

var builder = WebApplication.CreateBuilder(args);

// --- Inizio Sezione Servizi ---

// Definisci un nome per la policy CORS per poterla riutilizzare
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173") // L'URL del tuo frontend React (server di sviluppo Vite)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                          // Per produzione, dovrai aggiungere l'URL effettivo del tuo frontend deployato
                          // Esempio: policy.WithOrigins("http://localhost:5173", "https://tua-app-meteo.com")
                      });
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weatherapp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Registra IHttpClientFactory
builder.Services.AddHttpClient();

// Registra il servizio di geocodifica
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

// Registra il servizio di autenticazione
builder.Services.AddScoped<IAuthService, AuthService>();

// Registra il servizio meteo
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Configurazione dell'autenticazione JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;
        if (string.IsNullOrEmpty(tokenKey))
            throw new Exception("AppSettings:Token is not configured in appsettings.json");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddEndpointsApiExplorer();

// Modifica AddSwaggerGen per supportare l'autorizzazione JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorizzazione JWT con Bearer token. Esempio: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// --- Fine Sezione Servizi ---

var app = builder.Build();

// --- Inizio Applicazione Migrazioni ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            app.Logger.LogInformation("Applying database migrations...");
            dbContext.Database.Migrate();
            app.Logger.LogInformation("Database migrations applied successfully.");
        }
        else
        {
            app.Logger.LogInformation("No pending database migrations to apply.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
// --- Fine Applicazione Migrazioni ---

// --- Inizio Configurazione Pipeline HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApp API V1");
        // Opzionale: per fare il logout da swagger e resettare il token
        // c.InjectStylesheet("/swagger-ui/custom.css"); // Se decidi di creare questo file
    });
    app.UseDeveloperExceptionPage(); // Aggiunto per vedere errori dettagliati in sviluppo
}
else
{
    // Potresti aggiungere un gestore di eccezioni globale qui per la produzione
    // app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANTE: Aggiungi UseCors QUI
app.UseCors(MyAllowSpecificOrigins);

// app.UseRouting(); // In .NET 6+ UseRouting è spesso implicito, ma se lo avessi, UseCors andrebbe prima.

// IMPORTANTE: L'ordine di UseAuthentication e UseAuthorization è corretto
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Fine Configurazione Pipeline HTTP ---

// Il codice per il custom.css di Swagger è un commento e può rimanere tale o essere rimosso
// se non lo usi.

app.Run();
