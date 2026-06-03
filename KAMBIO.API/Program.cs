// PROGRAM.CS - Punto de entrada de la aplicación
// Aquí se configura todo: la BD, los servicios, los controladores, etc.
// Es como el "interruptor general" que prende el API.

using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;       // Para KambioDbContext (la BD)
using KAMBIO.CORE.Core.Interfaces;      // Para las interfaces (contratos)
using KAMBIO.CORE.Core.Services;        // Para los servicios (lógica de negocio)
using KAMBIO.CORE.Infrastructure.Repositories; // Para los repositorios (acceso a BD)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  // Habilita los controladores (endpoints)
builder.Services.AddOpenApi();      // Habilita Swagger (la documentación del API)

// Configura la conexión a SQL Server usando la cadena de conexión del appsettings.json
builder.Services.AddDbContext<KambioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// REGISTRO DE DEPENDENCIAS (Inyección de Dependencias)
// Le decimos a C#: "Cuando alguien pida IAlertaRepository, dale AlertaRepository"
// y "Cuando alguien pida IAlertaService, dale AlertaService"
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAlertaService, AlertaService>();

var app = builder.Build();  // Construye la aplicación

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  // Muestra Swagger solo en desarrollo
}

app.UseAuthorization(); // Habilita autorización (para cuando tengas login)
app.MapControllers();   // Conecta las rutas de los controladores

app.Run();  // Inicia el servidor web
