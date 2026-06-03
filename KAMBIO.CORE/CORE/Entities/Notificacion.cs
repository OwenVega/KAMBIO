using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    public int IdTipoNotificacion { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public bool Leida { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaLectura { get; set; }

    public int? IdReferencia { get; set; }

    public string? TipoReferencia { get; set; }

    public virtual TipoNotificacion IdTipoNotificacionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
