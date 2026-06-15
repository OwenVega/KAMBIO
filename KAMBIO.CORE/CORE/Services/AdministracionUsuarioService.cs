using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class AdministracionUsuarioService : IAdministracionUsuarioService
    {
        private readonly IAdministracionUsuarioRepository _adminRepo;

        public AdministracionUsuarioService(IAdministracionUsuarioRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<IEnumerable<UsuarioListadoAdminDto>> ObtenerListadoUsuariosAsync()
        {
            var usuarios = await _adminRepo.ObtenerUsuariosParaAdminAsync();
            var dtoList = new List<UsuarioListadoAdminDto>();

            foreach (var u in usuarios)
            {
                dtoList.Add(new UsuarioListadoAdminDto
                {
                    IdUsuario = u.IdUsuario,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Correo = u.Correo,
                    CalificacionPromedio = u.CalificacionPromedio,
                    TotalOrdenes = u.TotalOrdenes,
                    EstadoCuenta = u.IdEstadoCuentaNavigation.Nombre
                });
            }

            return dtoList;
        }

        public async Task CambiarEstadoCuentaAsync(CambiarEstadoUsuarioDto dto)
        {
            var usuario = await _adminRepo.ObtenerUsuarioPorIdAsync(dto.IdUsuarioObjetivo);
            if (usuario == null)
                throw new InvalidOperationException("El usuario objetivo no existe.");

            // Actualizamos los campos de auditoría exigidos en la US-019
            usuario.IdEstadoCuenta = dto.NuevoIdEstadoCuenta;
            usuario.MotivoBloqueo = dto.Motivo;
            usuario.FechaBloqueo = DateTime.Now;
            usuario.IdAdminBloqueo = dto.IdAdmin;

            await _adminRepo.ActualizarUsuarioAsync(usuario);

            // Si el estado es 2 (Suspendido) o 3 (Bloqueado), ejecutamos las acciones colaterales
            if (dto.NuevoIdEstadoCuenta == 2 || dto.NuevoIdEstadoCuenta == 3)
            {
                // Cancelamos ofertas activas
                await _adminRepo.CancelarOfertasActivasAsync(usuario.IdUsuario);

                // Mandamos a revisión transacciones activas
                await _adminRepo.MarcarTransaccionesParaRevisionAsync(usuario.IdUsuario);
            }
        }
    }
}