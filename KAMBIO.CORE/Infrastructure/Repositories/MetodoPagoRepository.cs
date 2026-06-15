using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly KambioDbContext _context;

        public MetodoPagoRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MetodoPago>> ObtenerPorUsuarioIdAsync(int idUsuario)
        {
            return await _context.MetodoPago
                .Include(m => m.IdBancoNavigation)
                .Where(m => m.IdUsuario == idUsuario && m.Activo == true)
                .ToListAsync();
        }

        public async Task<MetodoPago> ObtenerPorIdAsync(int idMetodoPago)
        {
            return await _context.MetodoPago.FindAsync(idMetodoPago);
        }

        public async Task AgregarAsync(MetodoPago metodoPago)
        {
            await _context.MetodoPago.AddAsync(metodoPago);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(MetodoPago metodoPago)
        {
            _context.MetodoPago.Update(metodoPago);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneTransaccionesActivasAsync(int idUsuario)
        {
            
            return await _context.Transaccion.AnyAsync(t =>
                (t.IdUsuarioComprador == idUsuario || t.IdUsuarioVendedor == idUsuario) &&
                (t.IdEstadoTransaccion == 1 || t.IdEstadoTransaccion == 2));
        }
    }
}