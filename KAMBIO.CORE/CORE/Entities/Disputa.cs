using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Disputa
{
    public int IdDisputa { get; set; }

    public int IdTransaccion { get; set; }

    public int IdUsuarioReporta { get; set; }

    public int IdEstadoDisputa { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateTime FechaReporte { get; set; }

    public DateTime? FechaResolucion { get; set; }

    public int? IdAdminResolucion { get; set; }

    public string? ResolucionDetalle { get; set; }

    public virtual Usuario? IdAdminResolucionNavigation { get; set; }

    public virtual EstadoDisputa IdEstadoDisputaNavigation { get; set; } = null!;

    public virtual Transaccion IdTransaccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioReportaNavigation { get; set; } = null!;
}
