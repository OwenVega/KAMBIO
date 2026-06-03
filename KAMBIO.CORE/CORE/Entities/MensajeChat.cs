using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class MensajeChat
{
    public int IdMensaje { get; set; }

    public int IdTransaccion { get; set; }

    public int IdUsuarioEnvia { get; set; }

    public string Mensaje { get; set; } = null!;

    public DateTime FechaEnvio { get; set; }

    public bool Leido { get; set; }

    public virtual Transaccion IdTransaccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioEnviaNavigation { get; set; } = null!;
}
