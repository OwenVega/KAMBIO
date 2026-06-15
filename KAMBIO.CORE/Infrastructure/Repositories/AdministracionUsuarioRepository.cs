using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class AdministracionUsuarioRepository : IAdministracionUsuarioRepository
    {
        private readonly KambioDbContext _context;

        public AdministracionUsuarioRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosParaAdminAsync()
        {
            // Solo trae usuarios con Rol = 1 (Usuarios normales, excluyendo a otros administradores)
            return await _context.Usuario
                .Include(u => u.IdEstadoCuentaNavigation)
                .Where(u => u.IdRol == 1)
                .ToListAsync();
        }

        public async Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            return await _context.Usuario.FindAsync(idUsuario);
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task CancelarOfertasActivasAsync(int idUsuario)
        {
            // Asumimos que IdEstadoOferta: 1 = Activa, 3 = Cancelada
            var ofertasActivas = await _context.Oferta
                .Where(o => o.IdUsuario == idUsuario && o.IdEstadoOferta == 1)
                .ToListAsync();

            foreach (var oferta in ofertasActivas)
            {
                oferta.IdEstadoOferta = 3; // Pasa a cancelada
                oferta.FechaCancelacion = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task MarcarTransaccionesParaRevisionAsync(int idUsuario)
        {
            // Asumimos Estados Transaccion: 1=Iniciada, 2=En Proceso, 5=En Revision Administrativa
            var transaccionesActivas = await _context.Transaccion
                .Where(t => (t.IdUsuarioComprador == idUsuario || t.IdUsuarioVendedor == idUsuario)
                         && (t.IdEstadoTransaccion == 1 || t.IdEstadoTransaccion == 2))
                .ToListAsync();

            foreach (var trx in transaccionesActivas)
            {
                trx.IdEstadoTransaccion = 5; // Pasa a Revisión
            }

            await _context.SaveChangesAsync();
        }
    }
}