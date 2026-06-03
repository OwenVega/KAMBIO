using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class VerificacionIdentidad
{
    public int IdVerificacion { get; set; }

    public int IdUsuario { get; set; }

    public int IdEstadoVerificacion { get; set; }

    public string RutaImagen { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaResolucion { get; set; }

    public int? IdAdminResolucion { get; set; }

    public string? ObservacionAdmin { get; set; }

    public virtual Usuario? IdAdminResolucionNavigation { get; set; }

    public virtual EstadoVerificacion IdEstadoVerificacionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
