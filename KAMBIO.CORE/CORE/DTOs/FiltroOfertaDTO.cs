using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.DTOs
{
    /// <summary>
    /// DTO para recibir los parámetros de búsqueda. Todos son opcionales (nullable)
    /// para permitir combinaciones dinámicas o limpiar los filtros.
    /// </summary>
    public class FiltroOfertaRequestDto
    {
        public int? IdTipoOferta { get; set; } // 1: Compra, 2: Venta
        public int? IdDivisaOrigen { get; set; }
        public int? IdDivisaDestino { get; set; }
        public int? IdBanco { get; set; }

        // Criterio: Rango de monto. Representa el monto exacto que el usuario quiere cambiar
        public decimal? MontoRequerido { get; set; }

        // Criterio: Rango de reputación
        public decimal? ReputacionMinima { get; set; }
        public decimal? ReputacionMaxima { get; set; }
    }

    /// <summary>
    /// DTO para mostrar los datos procesados de cada oferta compatible en el listado.
    /// </summary>
    public class OfertaFiltradaDto
    {
        public int IdOferta { get; set; }
        public string TipoOperacion { get; set; } = null!;
        public string Anunciante { get; set; } = null!;
        public decimal Reputacion { get; set; }
        public string MonedaOrigen { get; set; } = null!;
        public string MonedaDestino { get; set; } = null!;
        public decimal TasaCambio { get; set; }
        public decimal MontoDisponible { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public List<string> BancosAceptados { get; set; } = new List<string>();
        public DateTime FechaPublicacion { get; set; }
    }

    /// <summary>
    /// DTO contenedor que incluye el listado y el contador exigido por la US-020.
    /// </summary>
    public class FiltroOfertaResponseDto
    {
        public int TotalResultados { get; set; }
        public List<OfertaFiltradaDto> Ofertas { get; set; } = new List<OfertaFiltradaDto>();
    }
}