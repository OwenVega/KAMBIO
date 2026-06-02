using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class MetodoPago
{
    public int IdMetodoPago { get; set; }

    public int IdUsuario { get; set; }

    public int IdBanco { get; set; }

    public string TipoCuenta { get; set; } = null!;

    public string NumeroCuenta { get; set; } = null!;

    public string? Cci { get; set; }

    public string? Alias { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Banco IdBancoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
