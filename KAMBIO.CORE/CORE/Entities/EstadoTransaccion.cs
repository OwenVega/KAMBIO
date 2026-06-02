using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class EstadoTransaccion
{
    public int IdEstadoTransaccion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<HistorialEstadoTransaccion> HistorialEstadoTransaccion { get; set; } = new List<HistorialEstadoTransaccion>();

    public virtual ICollection<Transaccion> Transaccion { get; set; } = new List<Transaccion>();
}
