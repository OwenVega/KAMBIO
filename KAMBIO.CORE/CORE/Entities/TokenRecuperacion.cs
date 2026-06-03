using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class TokenRecuperacion
{
    public int IdToken { get; set; }

    public int IdUsuario { get; set; }

    public string Token { get; set; } = null!;

    public DateTime FechaExpiracion { get; set; }

    public bool Usado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
