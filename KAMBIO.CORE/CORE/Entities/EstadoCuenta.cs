using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class EstadoCuenta
{
    public int IdEstadoCuenta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
