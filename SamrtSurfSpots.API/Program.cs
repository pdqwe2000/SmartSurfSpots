using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartSurfSpots.Data;
using SmartSurfSpots.Data.Repositories;
using SmartSurfSpots.Services.Helpers;
using SmartSurfSpots.Services.Implementations;
using SmartSurfSpots.Services.Interfaces;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
    var xmlFileApi = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPathApi = Path.Combine(AppContext.BaseDirectory, xmlFileApi);
    c.IncludeXmlComments(xmlPathApi);

    var xmlFileDomain = "SmartSurfSpots.Domain.xml";
    var xmlPathDomain = Path.Combine(AppContext.BaseDirectory, xmlFileDomain);

    // O 'true' no segundo parâmetro ativa os comentários nos Schemas (DTOs)
    if (File.Exists(xmlPathDomain))
    {
        c.IncludeXmlComments(xmlPathDomain, includeControllerXmlComments: true);
    }
});

// Configurar DbContext
builder.Services.AddDbContext<SurfDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Registar Helpers
builder.Services.AddScoped<JwtHelper>();

// Configurar CORS (opcional, útil para frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();