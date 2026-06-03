using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class MatchOferta
{
    public int IdMatch { get; set; }

    public int IdOfertaOrigen { get; set; }

    public int IdOfertaMatch { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaMatch { get; set; }

    public DateTime? FechaRespuesta { get; set; }

    public virtual Oferta IdOfertaMatchNavigation { get; set; } = null!;

    public virtual Oferta IdOfertaOrigenNavigation { get; set; } = null!;
}
