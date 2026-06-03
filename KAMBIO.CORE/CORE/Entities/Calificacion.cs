using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Calificacion
{
    public int IdCalificacion { get; set; }

    public int IdTransaccion { get; set; }

    public int IdUsuarioEvalua { get; set; }

    public int IdUsuarioEvaluado { get; set; }

    public byte Estrellas { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaCalificacion { get; set; }

    public virtual Transaccion IdTransaccionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioEvaluaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioEvaluadoNavigation { get; set; } = null!;
}
