using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Divisa
{
    public int IdDivisa { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Simbolo { get; set; } = null!;

    public virtual ICollection<AlertaTipoCambio> AlertaTipoCambioIdDivisaDestinoNavigation { get; set; } = new List<AlertaTipoCambio>();

    public virtual ICollection<AlertaTipoCambio> AlertaTipoCambioIdDivisaOrigenNavigation { get; set; } = new List<AlertaTipoCambio>();

    public virtual ICollection<Oferta> OfertaIdDivisaDestinoNavigation { get; set; } = new List<Oferta>();

    public virtual ICollection<Oferta> OfertaIdDivisaOrigenNavigation { get; set; } = new List<Oferta>();

    public virtual ICollection<Transaccion> TransaccionIdDivisaDestinoNavigation { get; set; } = new List<Transaccion>();

    public virtual ICollection<Transaccion> TransaccionIdDivisaOrigenNavigation { get; set; } = new List<Transaccion>();
}
