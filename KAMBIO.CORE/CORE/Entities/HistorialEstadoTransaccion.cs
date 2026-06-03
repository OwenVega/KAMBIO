using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class HistorialEstadoTransaccion
{
    public int IdHistorial { get; set; }

    public int IdTransaccion { get; set; }

    public int IdEstadoTransaccion { get; set; }

    public DateTime FechaCambio { get; set; }

    public string? Observacion { get; set; }

    public int IdUsuarioCambio { get; set; }

    public virtual EstadoTransaccion IdEstadoTransaccionNavigation { get; set; } = null!;

    public virtual Transaccion IdTransaccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCambioNavigation { get; set; } = null!;
}
