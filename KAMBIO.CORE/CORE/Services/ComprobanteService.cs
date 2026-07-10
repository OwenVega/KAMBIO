using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace KAMBIO.CORE.Core.Services
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly KambioDbContext _context;

        public ComprobanteService(KambioDbContext context)
        {
            _context = context;
        }
        public async Task<List<ComprobanteDto>> ObtenerPorTransaccionAsync(int idTransaccion)
        {
            var comprobantes = await _context.Comprobante
                .Where(c => c.IdTransaccion == idTransaccion && c.Activo)
                .OrderByDescending(c => c.FechaSubida)
                .ToListAsync();

            return comprobantes.Select(c => new ComprobanteDto
            {
                IdComprobante = c.IdComprobante,
                RutaImagen = c.RutaImagen,
                FechaSubida = c.FechaSubida
            }).ToList();
        }
      
        public async Task SubirComprobanteAsync(int idTransaccion, int idUsuario, IFormFile archivo, string carpetaVouchers)
        {
            var transaccion = await _context.Transaccion.FindAsync(idTransaccion)
                ?? throw new InvalidOperationException("Transacción no encontrada.");

            var ext = Path.GetExtension(archivo.FileName).ToLower();
            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png" };
            if (!extensionesValidas.Contains(ext))
                throw new InvalidOperationException("Solo se permiten archivos JPG o PNG.");

            Directory.CreateDirectory(carpetaVouchers);

            var nombreArchivo = $"{DateTime.Now:yyyy-MM-dd_HHmmss}_transaccion{idTransaccion}{ext}";
            var rutaCompleta = Path.Combine(carpetaVouchers, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                await archivo.CopyToAsync(stream);

            var comprobante = new Comprobante
            {
                IdTransaccion = idTransaccion,
                IdUsuario = idUsuario,
                RutaImagen = $"/vouchers/{nombreArchivo}",
                FechaSubida = DateTime.Now,
                Activo = true
            };
            _context.Comprobante.Add(comprobante);

            transaccion.FechaConfirmacionPago = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}