using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class EstadoDisputa
{
    public int IdEstadoDisputa { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Disputa> Disputa { get; set; } = new List<Disputa>();
}
