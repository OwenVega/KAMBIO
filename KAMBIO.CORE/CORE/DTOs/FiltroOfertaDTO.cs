using System;
using System.Collections.Generic;
using System.Text;

namespace KAMBIO.CORE.Core.DTOs
{
    public class FiltroOfertaRequestDto
    {
        public int? IdTipoOferta { get; set; }
        public int? IdDivisaOrigen { get; set; }
        public int? IdDivisaDestino { get; set; }
        public int? IdBanco { get; set; }
        public decimal? MontoRequerido { get; set; }
        public decimal? ReputacionMinima { get; set; }
        public decimal? ReputacionMaxima { get; set; }
    }

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

    public class FiltroOfertaResponseDto
    {
        public int TotalResultados { get; set; }
        public List<OfertaFiltradaDto> Ofertas { get; set; } = new List<OfertaFiltradaDto>();
    }

    public class FiltroOfertaDTO
    {
        public int IdTipoOferta { get; set; }
        public int IdDivisaOrigen { get; set; }
        public int IdDivisaDestino { get; set; }
        public decimal? Monto { get; set; }
        public int? IdBanco { get; set; }
    }
}