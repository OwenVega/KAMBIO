using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class MatchSugeridoDto
    {
        public int IdMatch { get; set; }
        public int IdOfertaContraparte { get; set; }

       
        public string Anunciante { get; set; } = null!;
        public decimal Reputacion { get; set; }
        public int TotalOperaciones { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal MontoDisponible { get; set; }
        public List<string> MetodosPagoAceptados { get; set; } = new List<string>();
        public DateTime FechaPublicacionOferta { get; set; }
    }

    public class RespuestaMatchDto
    {
        [Required(ErrorMessage = "El ID del match es obligatorio.")]
        public int IdMatch { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe indicar si acepta o rechaza la coincidencia.")]
        public bool Aceptado { get; set; }
    }
}