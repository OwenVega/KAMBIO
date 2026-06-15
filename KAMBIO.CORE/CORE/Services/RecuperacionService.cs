using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.CORE.Services
{
    public class RecuperacionService : IRecuperacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRecuperacionRepository _tokenRepository;

        public RecuperacionService(
            IUsuarioRepository usuarioRepository,
            ITokenRecuperacionRepository tokenRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task SolicitarRecuperacionAsync(string correo)
        {
            var existe = await _usuarioRepository.ExisteCorreo(correo);
            if (!existe)
                throw new InvalidOperationException("No existe una cuenta con ese correo.");

            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);

            var token = new TokenRecuperacion
            {
                IdUsuario = usuario.IdUsuario,
                Token = Guid.NewGuid().ToString("N"),
                FechaExpiracion = DateTime.UtcNow.AddMinutes(30),
                Usado = false,
                FechaCreacion = DateTime.UtcNow
            };

            await _tokenRepository.CrearTokenAsync(token);

            // TODO: integrar SMTP/SendGrid para envío real
            Console.WriteLine($"[DEV] Token generado: {token.Token}");
        }

        public async Task RestablecerContrasenaAsync(string token, string nuevaContrasena, string confirmarContrasena)
        {
            if (nuevaContrasena.Length < 8)
                throw new InvalidOperationException("La contrasena debe tener al menos 8 caracteres.");

            if (nuevaContrasena != confirmarContrasena)
                throw new InvalidOperationException("Las contrasenas no coinciden.");

            var tokenEntity = await _tokenRepository.ObtenerTokenValidoAsync(token);
            if (tokenEntity == null)
                throw new InvalidOperationException("El enlace de recuperacion es invalido o ha expirado.");

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(tokenEntity.IdUsuario);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            await _usuarioRepository.ActualizarAsync(usuario);

            await _tokenRepository.MarcarTokenUsadoAsync(tokenEntity.IdToken);
        }
    }
}
