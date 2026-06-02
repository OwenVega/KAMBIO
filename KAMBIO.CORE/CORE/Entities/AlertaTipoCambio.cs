using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class AlertaTipoCambio
{
    public int IdAlerta { get; set; }

    public int IdUsuario { get; set; }

    public int IdDivisaOrigen { get; set; }

    public int IdDivisaDestino { get; set; }

    public decimal ValorUmbral { get; set; }

    public bool Activa { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaDisparo { get; set; }

    public virtual Divisa IdDivisaDestinoNavigation { get; set; } = null!;

    public virtual Divisa IdDivisaOrigenNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
