using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Banco
{
    public int IdBanco { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public virtual ICollection<MetodoPago> MetodoPago { get; set; } = new List<MetodoPago>();

    public virtual ICollection<OfertaMetodoPago> OfertaMetodoPago { get; set; } = new List<OfertaMetodoPago>();
}
