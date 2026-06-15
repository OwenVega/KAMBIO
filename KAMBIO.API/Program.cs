// PROGRAM.CS - Punto de entrada
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;
using KAMBIO.CORE.Core.Services;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.CORE.Services;
using KAMBIO.CORE.Infrastructure.Data;
using KAMBIO.CORE.Infrastructure.Repositories;
using KAMBIO.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

var _config = builder.Configuration;
var cnx = _config.GetConnectionString("DevConnection");

builder.Services.AddDbContext<KambioDbContext>(options =>
    options.UseSqlServer(cnx));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IFiltroOfertaRepository, FiltroOfertaRepository>();
builder.Services.AddScoped<IFiltroOfertaService, FiltroOfertaService>();
builder.Services.AddScoped<IAdministracionUsuarioRepository, AdministracionUsuarioRepository>();
builder.Services.AddScoped<IAdministracionUsuarioService, AdministracionUsuarioService>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IOfertaService, OfertaService>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<IComprobanteService, ComprobanteService>();
builder.Services.AddScoped<ICalificacionService, CalificacionService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IDisputaRepository, DisputaRepository>();
builder.Services.AddScoped<IDisputaService, DisputaService>();
builder.Services.AddScoped<IConfirmacionPagoRepository, ConfirmacionPagoRepository>();
builder.Services.AddScoped<IConfirmacionPagoService, ConfirmacionPagoService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IRecuperacionService, RecuperacionService>();
builder.Services.AddScoped<ITokenRecuperacionRepository, TokenRecuperacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IOfertaVentaRepository, OfertaVentaRepository>();
builder.Services.AddScoped<IOfertaVentaService, OfertaVentaService>();
builder.Services.AddScoped<IMensajeChatRepository, MensajeChatRepository>();
builder.Services.AddScoped<IMensajeChatService, MensajeChatService>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificacionHub>("/hubs/notificaciones");

app.Run();