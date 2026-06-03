using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Comprobante
{
    public int IdComprobante { get; set; }

    public int IdTransaccion { get; set; }

    public int IdUsuario { get; set; }

    public string RutaImagen { get; set; } = null!;

    public DateTime FechaSubida { get; set; }

    public bool Activo { get; set; }

    public virtual Transaccion IdTransaccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
