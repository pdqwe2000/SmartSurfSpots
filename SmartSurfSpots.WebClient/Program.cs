var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Servir ficheiros estáticos (HTML, CSS, JS)
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();