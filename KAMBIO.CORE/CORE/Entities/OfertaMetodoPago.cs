using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class OfertaMetodoPago
{
    public int IdOfertaMetodoPago { get; set; }

    public int IdOferta { get; set; }

    public int IdBanco { get; set; }

    public virtual Banco IdBancoNavigation { get; set; } = null!;

    public virtual Oferta IdOfertaNavigation { get; set; } = null!;
}
