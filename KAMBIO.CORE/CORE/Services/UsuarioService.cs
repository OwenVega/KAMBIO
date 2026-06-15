using System;
using System.Threading.Tasks;
using BCrypt.Net;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task RegistrarUsuarioAsync(RegistroUsuarioDto dto)
        {

            bool correoExiste = await _usuarioRepository.ExisteCorreo(dto.Correo);
            if (correoExiste)
            {
                throw new InvalidOperationException("Este correo ya está registrado.");
            }

            string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var nuevoUsuario = new Usuario
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Correo = dto.Correo,
                PasswordHash = passwordHasheada,
                IdRol = 1,          
                IdEstadoCuenta = 1  
            };

            await _usuarioRepository.agregarUsuario(nuevoUsuario);
        }

        public async Task<Usuario> LoginAsync(LoginUsuarioDto dto)
        {

            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(dto.Correo);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Correo o contraseña incorrecta.");
            }

            bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.PasswordHash);

            if (!passwordCorrecta)
            {

                throw new UnauthorizedAccessException("Correo o contraseña incorrecta.");
            }


            if (usuario.IdEstadoCuenta == 2 || usuario.IdEstadoCuenta == 3)
            {
                throw new InvalidOperationException("Tu cuenta ha sido suspendida. Comunícate con soporte.");
            }

            return usuario;
        }
    }
}