using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.CORE.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public PerfilService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PerfilUsuarioDto> ObtenerPerfilAsync(int idUsuario)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);
            if (usuario == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            return new PerfilUsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                FotoPerfil = usuario.FotoPerfil,
                CalificacionPromedio = usuario.CalificacionPromedio
            };
        }

        public async Task ActualizarPerfilAsync(int idUsuario, ActualizarPerfilDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);
            if (usuario == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            usuario.Nombres = dto.Nombres;
            usuario.Apellidos = dto.Apellidos;
            usuario.Telefono = dto.Telefono;

            await _usuarioRepository.ActualizarAsync(usuario);
        }

        public async Task ActualizarFotoPerfilAsync(int idUsuario, string rutaFoto)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);
            if (usuario == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            usuario.FotoPerfil = rutaFoto;
            await _usuarioRepository.ActualizarAsync(usuario);
        }
    }
}