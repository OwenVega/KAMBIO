using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class EstadoOferta
{
    public int IdEstadoOferta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Oferta> Oferta { get; set; } = new List<Oferta>();
}
