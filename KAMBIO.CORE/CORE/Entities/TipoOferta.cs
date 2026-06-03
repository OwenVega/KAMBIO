using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class TipoOferta
{
    public int IdTipoOferta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Oferta> Oferta { get; set; } = new List<Oferta>();
}
