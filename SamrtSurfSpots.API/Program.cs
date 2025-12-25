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

// ==============================================================================
// 1. CONFIGURAÇÃO DE SERVIÇOS DO CORE (INJEÇÃO DE DEPENDÊNCIAS)
// ==============================================================================

// Adiciona suporte para Controllers (API Endpoints)
builder.Services.AddControllers();

// Adiciona o explorador de endpoints necessário para o Swagger
builder.Services.AddEndpointsApiExplorer();

// Configuração avançada do Swagger (Documentação da API)
builder.Services.AddSwaggerGen(c =>
{
    // Metadados básicos da API
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Smart Surf Spots API",
        Version = "v1",
        Description = "API para gestão de spots de surf com autenticação JWT e integração meteorológica."
    });

    // Configuração do botão "Authorize" (Cadeado) para testar endpoints protegidos
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Exige que o esquema de segurança acima seja usado globalmente
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

    // Carregamento dos Comentários XML da API (Controllers)
    var xmlFileApi = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPathApi = Path.Combine(AppContext.BaseDirectory, xmlFileApi);
    c.IncludeXmlComments(xmlPathApi);

    // Carregamento dos Comentários XML do Domain (DTOs)
    // Verifica se o ficheiro existe para evitar erros se o projeto Domain não tiver XML gerado
    var xmlFileDomain = "SmartSurfSpots.Domain.xml";
    var xmlPathDomain = Path.Combine(AppContext.BaseDirectory, xmlFileDomain);

    if (File.Exists(xmlPathDomain))
    {
        // O parâmetro 'includeControllerXmlComments: true' garante que as descrições dos DTOs apareçam
        c.IncludeXmlComments(xmlPathDomain, includeControllerXmlComments: true);
    }
});

// Configuração da Base de Dados (Entity Framework Core com SQL Server)
builder.Services.AddDbContext<SurfDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de Autenticação e JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Parâmetros de validação do Token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,             // Valida quem emitiu o token
        ValidateAudience = true,           // Valida para quem é o token
        ValidateLifetime = true,           // Valida se não expirou
        ValidateIssuerSigningKey = true,   // Valida a assinatura digital
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero          // Remove a tolerância de tempo padrão (5min) para expiração
    };
});

// Ativa o serviço de Autorização
builder.Services.AddAuthorization();

// ==============================================================================
// 2. REGISTO DE DEPENDÊNCIAS PERSONALIZADAS (IOC CONTAINER)
// ==============================================================================

// Repositórios (Acesso a Dados)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISpotRepository, SpotRepository>();

// Cliente HTTP (Para chamadas a APIs externas como Open-Meteo)
builder.Services.AddHttpClient();

// Serviços de Domínio (Lógica de Negócio)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISpotService, SpotService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Helpers e Utilitários
builder.Services.AddScoped<JwtHelper>();

// Configuração de CORS (Permitir acesso de qualquer origem - útil para desenvolvimento frontend)
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

// ==============================================================================
// 3. CONFIGURAÇÃO DO PIPELINE DE PEDIDOS HTTP (MIDDLEWARE)
// ==============================================================================

// Em ambiente de desenvolvimento, ativa a interface visual do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplica a política de CORS definida acima
app.UseCors("AllowAll");

// Middleware de Autenticação (Quem és tu?) e Autorização (O que podes fazer?)
// A ordem aqui é CRUCIAL: AuthN antes de AuthZ
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os endpoints dos Controllers para as rotas
app.MapControllers();

app.Run();