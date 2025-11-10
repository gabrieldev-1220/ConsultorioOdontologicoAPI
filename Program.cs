// Program.cs (CÓDIGO COMPLETO MODIFICADO - SOLO CORS)
using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Interfaces;
using ConsultorioOdontologicoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === CARGA DE CONFIGURACIÓN SEGÚN ENTORNO ===
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// === CONFIGURACIÓN DE JWT CON VALIDACIÓN ===
var jwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKeyBase64 = jwtConfig["Key"];
var jwtIssuer = jwtConfig["Issuer"];
var jwtAudience = jwtConfig["Audience"];
var expiryMinutes = 60;

if (string.IsNullOrEmpty(jwtKeyBase64) ||
    string.IsNullOrEmpty(jwtIssuer) ||
    string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("Configuración JWT incompleta en appsettings.json");
}

// Decodificar clave Base64
byte[] keyBytes;
try
{
    keyBytes = Convert.FromBase64String(jwtKeyBase64);
}
catch (FormatException)
{
    throw new InvalidOperationException("Jwt:Key debe estar en formato Base64 válido");
}
if (keyBytes.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key debe tener al menos 256 bits (32 bytes) después de decodificar");
}

// === SERVICIOS ===
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 32;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger con JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consultorio Odontologico API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer ' + token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Base de datos
builder.Services.AddDbContext<ConsultorioOdontologicoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)));

// Inyección de dependencias
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// CORS - ACTUALIZADO PARA NETLIFY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "https://coindental.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// === MIDDLEWARE ===
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Error interno del servidor.");
    });
});

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consultorio Odontologico API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();