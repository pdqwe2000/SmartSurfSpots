using Microsoft.EntityFrameworkCore;
using SmartSurfSpots.Data;
using SmartSurfSpots.SoapService.Contracts;
using SmartSurfSpots.SoapService.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<SurfDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registar o serviço SOAP
builder.Services.AddScoped<ISoapDataService, SoapDataService>();

// Necessário para SoapCore
builder.Services.AddMvc();

var app = builder.Build();

app.UseRouting();

// Configurar endpoint SOAP
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<ISoapDataService>("/SoapDataService.asmx", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
});

// Página de informação na raiz
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Smart Surf Spots - SOAP Service</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        h1 { color: #0066cc; }
        .endpoint { background: #f0f0f0; padding: 20px; border-radius: 5px; margin: 20px 0; }
        code { background: #e0e0e0; padding: 2px 6px; border-radius: 3px; }
    </style>
</head>
<body>
    <h1>Smart Surf Spots - SOAP Service</h1>
    <p>Serviço SOAP para operações de dados</p>
    
    <div class='endpoint'>
        <h2>WSDL Endpoint</h2>
        <p><code>GET /SoapDataService.asmx?wsdl</code></p>
        <p>Use este endpoint para obter o WSDL do serviço</p>
    </div>
    
    <div class='endpoint'>
        <h2>SOAP Endpoint</h2>
        <p><code>POST /SoapDataService.asmx</code></p>
        <p>Endpoint para chamadas SOAP</p>
    </div>
    
    <h3>Operações Disponíveis:</h3>
    <ul>
        <li><strong>Users:</strong> GetUserById, GetUserByEmail, GetAllUsers</li>
        <li><strong>Spots:</strong> GetSpotById, GetAllSpots, CreateSpot, UpdateSpot, DeleteSpot</li>
        <li><strong>CheckIns:</strong> CreateCheckIn, GetCheckInsBySpot, GetCheckInsByUser</li>
    </ul>
    
    <p><a href='/SoapDataService.asmx?wsdl'>Ver WSDL</a></p>
</body>
</html>
", "text/html"));

app.Run();