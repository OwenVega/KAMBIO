// PROGRAM.CS - Punto de entrada de la aplicación
// Aquí se configura la conexión a BD y se registran los servicios de Verificación.
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;
using KAMBIO.CORE.Core.Services;
using KAMBIO.CORE.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Conexión a SQL Server
builder.Services.AddDbContext<KambioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// Registro de dependencias para Verificación de Identidad
builder.Services.AddScoped<IVerificacionRepository, VerificacionRepository>();
builder.Services.AddScoped<IVerificacionService, VerificacionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();

app.Run();
