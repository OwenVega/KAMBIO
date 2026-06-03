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
            // Paso 1: Regla de negocio - Validar unicidad del correo
            bool correoExiste = await _usuarioRepository.ExisteCorreo(dto.Correo);
            if (correoExiste)
            {
                // Lanzamos una excepción controlada para que el controlador la atrape
                throw new InvalidOperationException("Este correo ya está registrado.");
            }

            // Paso 2: Seguridad - Encriptar la contraseña plana
            string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            // Paso 3: Mapeo - Convertir el DTO en la Entidad real
            var nuevoUsuario = new Usuario
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Correo = dto.Correo,
                PasswordHash = passwordHasheada,

                // Reglas de negocio obligatorias para usuarios nuevos
                IdRol = 1,          // 1 = 'Usuario' 
                IdEstadoCuenta = 1  // 1 = 'Activo' 
            };

            // Paso 4: Persistencia - Enviar la entidad al repositorio
            await _usuarioRepository.agregarUsuario(nuevoUsuario);
        }
    }
}