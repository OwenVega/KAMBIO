using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces; 
using KAMBIO.CORE.Infrastructure.Data;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly KambioDbContext _context;


        public UsuarioRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<Boolean> ExisteCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;

            return await _context.Usuario.AnyAsync(u => u.Correo == correo);
        }

        public async Task agregarUsuario(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            if (usuario.FechaRegistro == default) usuario.FechaRegistro = DateTime.Now;

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }
    }
}

