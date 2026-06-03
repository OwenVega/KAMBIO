using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Transaccion
{
    public int IdTransaccion { get; set; }

    public int IdOferta { get; set; }

    public int IdUsuarioComprador { get; set; }

    public int IdUsuarioVendedor { get; set; }

    public int IdEstadoTransaccion { get; set; }

    public int IdDivisaOrigen { get; set; }

    public int IdDivisaDestino { get; set; }

    public decimal Monto { get; set; }

    public decimal MontoEquivalente { get; set; }

    public decimal TasaCambioAplicada { get; set; }

    public string TipoOperacion { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaConfirmacionPago { get; set; }

    public DateTime? FechaCompletado { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public bool ConfirmadoPorComprador { get; set; }

    public bool ConfirmadoPorVendedor { get; set; }

    public virtual ICollection<Calificacion> Calificacion { get; set; } = new List<Calificacion>();

    public virtual ICollection<Comprobante> Comprobante { get; set; } = new List<Comprobante>();

    public virtual ICollection<Disputa> Disputa { get; set; } = new List<Disputa>();

    public virtual ICollection<HistorialEstadoTransaccion> HistorialEstadoTransaccion { get; set; } = new List<HistorialEstadoTransaccion>();

    public virtual Divisa IdDivisaDestinoNavigation { get; set; } = null!;

    public virtual Divisa IdDivisaOrigenNavigation { get; set; } = null!;

    public virtual EstadoTransaccion IdEstadoTransaccionNavigation { get; set; } = null!;

    public virtual Oferta IdOfertaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCompradorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioVendedorNavigation { get; set; } = null!;

    public virtual ICollection<MensajeChat> MensajeChat { get; set; } = new List<MensajeChat>();
}
