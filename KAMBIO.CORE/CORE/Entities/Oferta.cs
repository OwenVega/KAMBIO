using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Oferta
{
    public int IdOferta { get; set; }

    public int IdUsuario { get; set; }

    public int IdTipoOferta { get; set; }

    public int IdEstadoOferta { get; set; }

    public int IdDivisaOrigen { get; set; }

    public int IdDivisaDestino { get; set; }

    public decimal MontoDisponible { get; set; }

    public decimal MontoMinimo { get; set; }

    public decimal MontoMaximo { get; set; }

    public decimal TasaCambio { get; set; }

    public DateTime FechaPublicacion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public DateTime? FechaCompletado { get; set; }

    public virtual Divisa IdDivisaDestinoNavigation { get; set; } = null!;

    public virtual Divisa IdDivisaOrigenNavigation { get; set; } = null!;

    public virtual EstadoOferta IdEstadoOfertaNavigation { get; set; } = null!;

    public virtual TipoOferta IdTipoOfertaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<MatchOferta> MatchOfertaIdOfertaMatchNavigation { get; set; } = new List<MatchOferta>();

    public virtual ICollection<MatchOferta> MatchOfertaIdOfertaOrigenNavigation { get; set; } = new List<MatchOferta>();

    public virtual ICollection<OfertaMetodoPago> OfertaMetodoPago { get; set; } = new List<OfertaMetodoPago>();

    public virtual ICollection<Transaccion> Transaccion { get; set; } = new List<Transaccion>();
}
