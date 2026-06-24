using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class TokenRecuperacionRepository : ITokenRecuperacionRepository
    {
        private readonly KambioDbContext _context;

        public TokenRecuperacionRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task CrearTokenAsync(TokenRecuperacion token)
        {
            _context.TokenRecuperacion.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenRecuperacion?> ObtenerTokenValidoAsync(string token)
        {
            return await _context.TokenRecuperacion
                .FirstOrDefaultAsync(t => t.Token == token
                                       && !t.Usado
                                       && t.FechaExpiracion > DateTime.UtcNow);
        }

        public async Task MarcarTokenUsadoAsync(int idToken)
        {
            var token = await _context.TokenRecuperacion.FindAsync(idToken);
            if (token != null)
            {
                token.Usado = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}