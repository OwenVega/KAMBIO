using KAMBIO.CORE.Core.Services;
using KAMBIO.CORE.CORE.Services;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.CORE.Services;
using KAMBIO.CORE.Infrastructure.Data;
using KAMBIO.CORE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var _config = builder.Configuration;
var cnx = _config.GetConnectionString("DevConnection");

builder.Services.AddDbContext<KambioDbContext>(options =>
  options.UseSqlServer(cnx));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IOfertaService, OfertaService>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();

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

app.Run();