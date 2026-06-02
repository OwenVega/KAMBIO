using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class EstadoVerificacion
{
    public int IdEstadoVerificacion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<VerificacionIdentidad> VerificacionIdentidad { get; set; } = new List<VerificacionIdentidad>();
}
