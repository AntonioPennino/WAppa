// Program.cs
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Services; // Aggiungi questo per IAuthService e AuthService
using Microsoft.AspNetCore.Authentication.JwtBearer; // Aggiungi questo
using Microsoft.IdentityModel.Tokens; // Aggiungi questo
using System.Text; // Aggiungi questo
using Microsoft.OpenApi.Models; // Per la configurazione di Swagger per JWT
using WeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Inizio Sezione Servizi ---

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weatherapp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Registra IHttpClientFactory
builder.Services.AddHttpClient(); // Registrazione base

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
            ValidateIssuer = false, // Per semplicità, non validiamo l'issuer in questo esempio
            ValidateAudience = false // Per semplicità, non validiamo l'audience in questo esempio
        };
    });

builder.Services.AddEndpointsApiExplorer();

// Modifica AddSwaggerGen per supportare l'autorizzazione JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApp API", Version = "v1" });

    // Definisci lo schema di sicurezza per JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorizzazione JWT con Bearer token. Esempio: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Aggiungi un requisito di sicurezza globale per usare lo schema Bearer
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

// --- Inizio Configurazione Pipeline HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApp API V1");
        // Opzionale: per fare il logout da swagger e resettare il token
        c.InjectStylesheet("/swagger-ui/custom.css"); // (dovrai creare questo file css)
    });
}

app.UseHttpsRedirection();

// IMPORTANTE: Aggiungi queste righe ORA
app.UseAuthentication(); // Deve venire PRIMA di UseAuthorization
app.UseAuthorization();

app.MapControllers();

// --- Fine Configurazione Pipeline HTTP ---

// Opzionale: crea un file wwwroot/swagger-ui/custom.css per il pulsante di logout da Swagger
// Se non vuoi questa funzionalità puoi omettere l'InjectStylesheet e la creazione del file
// Esempio di custom.css:
// .swagger-ui .topbar .download-url-wrapper { display: none }
// .swagger-ui .topbar-wrapper a span { display: none }
// .swagger-ui .topbar-wrapper a[href*="swagger.json"] { content: "API Specification"; }
// .swagger-ui .auth-wrapper .authorize.locked { background-color: #4CAF50; }
// .swagger-ui .auth-wrapper .authorize.unlocked { background-color: #f44336; }


app.Run();
