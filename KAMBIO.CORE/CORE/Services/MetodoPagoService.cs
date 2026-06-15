using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoPagoRepository _metodoPagoRepository;

        public MetodoPagoService(IMetodoPagoRepository metodoPagoRepository)
        {
            _metodoPagoRepository = metodoPagoRepository;
        }

        public async Task<IEnumerable<MetodoPagoListDto>> ObtenerMetodosPagoUsuarioAsync(int idUsuario)
        {
            var metodos = await _metodoPagoRepository.ObtenerPorUsuarioIdAsync(idUsuario);
            var dtoList = new List<MetodoPagoListDto>();

            foreach (var metodo in metodos)
            {
                dtoList.Add(new MetodoPagoListDto
                {
                    IdMetodoPago = metodo.IdMetodoPago,
                    Banco = metodo.IdBancoNavigation.Nombre,
                    TipoCuenta = metodo.TipoCuenta,
                    
                    NumeroCuentaEnmascarado = EnmascararCuenta(metodo.NumeroCuenta),
                    Activo = metodo.Activo
                });
            }

            return dtoList;
        }

        public async Task RegistrarMetodoPagoAsync(MetodoPagoCrearDto dto)
        {
            var nuevoMetodo = new MetodoPago
            {
                IdUsuario = dto.IdUsuario,
                IdBanco = dto.IdBanco,
                TipoCuenta = dto.TipoCuenta,
                NumeroCuenta = dto.NumeroCuenta,
                Cci = dto.Cci,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            await _metodoPagoRepository.AgregarAsync(nuevoMetodo);
        }

        public async Task EliminarMetodoPagoAsync(int idMetodoPago, int idUsuario)
        {
            var metodo = await _metodoPagoRepository.ObtenerPorIdAsync(idMetodoPago);
            if (metodo == null || metodo.IdUsuario != idUsuario)
                throw new InvalidOperationException("El método de pago no existe o no le pertenece.");

            
            bool tieneOperacionesActivas = await _metodoPagoRepository.TieneTransaccionesActivasAsync(idUsuario);
            if (tieneOperacionesActivas)
            {
                throw new InvalidOperationException("No se puede eliminar la cuenta bancaria porque tiene operaciones P2P activas o pendientes asociadas.");
            }

           
            metodo.Activo = false;
            await _metodoPagoRepository.ActualizarAsync(metodo);
        }

        private string EnmascararCuenta(string numeroCuenta)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta) || numeroCuenta.Length <= 4)
                return numeroCuenta;

            return "****" + numeroCuenta.Substring(numeroCuenta.Length - 4);
        }
    }
}