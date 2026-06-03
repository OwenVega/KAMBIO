using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class TipoNotificacion
{
    public int IdTipoNotificacion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificacion { get; set; } = new List<Notificacion>();
}
