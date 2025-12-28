using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartSurfSpots.Data;
using SmartSurfSpots.Data.Repositories;
using SmartSurfSpots.Services.Helpers;
using SmartSurfSpots.Services.Implementations;
using SmartSurfSpots.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar porta do Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger com suporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Smart Surf Spots API",
        Version = "v1",
        Description = "API para gestão de spots de surf com autenticação JWT"
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Configurar DbContext (PostgreSQL para Railway, SQL Server para local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var usePostgres = builder.Configuration.GetValue<bool>("UsePostgreSQL");

if (usePostgres || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")))
{
    // Railway PostgreSQL
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        connectionString = ConvertPostgresUrl(databaseUrl);
    }

    builder.Services.AddDbContext<SurfDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // SQL Server local
    builder.Services.AddDbContext<SurfDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Registar Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISpotRepository, SpotRepository>();

// Registar HttpClient para APIs externas
builder.Services.AddHttpClient();

// Registar Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISpotService, SpotService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();

// Registar Helpers
builder.Services.AddScoped<JwtHelper>();

// Configurar CORS (permitir WebClient)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://smartsurfspots-web-production.up.railway.app", "https://localhost:7109") // Ajustar portas do WebClient
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Aplicar migrations automaticamente no startup (apenas em produção)
if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<SurfDbContext>();
            db.Database.Migrate();
            Console.WriteLine("✓ Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error applying migrations: {ex.Message}");
        }
    }
}

// Configure the HTTP request pipeline.
// Ativar Swagger em todos os ambientes (desenvolvimento e produção)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Surf Spots API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz (https://localhost:XXXX/)
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Converter URL do Railway PostgreSQL para connection string
static string ConvertPostgresUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}