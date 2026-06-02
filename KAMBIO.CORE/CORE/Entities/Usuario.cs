using System;
using System.Collections.Generic;

namespace KAMBIO.CORE.Core.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public int IdEstadoCuenta { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? FotoPerfil { get; set; }

    public bool EsVerificado { get; set; }

    public decimal CalificacionPromedio { get; set; }

    public int TotalOrdenes { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaUltimaConexion { get; set; }

    public string? MotivoBloqueo { get; set; }

    public DateTime? FechaBloqueo { get; set; }

    public int? IdAdminBloqueo { get; set; }

    public virtual ICollection<AlertaTipoCambio> AlertaTipoCambio { get; set; } = new List<AlertaTipoCambio>();

    public virtual ICollection<Calificacion> CalificacionIdUsuarioEvaluaNavigation { get; set; } = new List<Calificacion>();

    public virtual ICollection<Calificacion> CalificacionIdUsuarioEvaluadoNavigation { get; set; } = new List<Calificacion>();

    public virtual ICollection<Comprobante> Comprobante { get; set; } = new List<Comprobante>();

    public virtual ICollection<Disputa> DisputaIdAdminResolucionNavigation { get; set; } = new List<Disputa>();

    public virtual ICollection<Disputa> DisputaIdUsuarioReportaNavigation { get; set; } = new List<Disputa>();

    public virtual ICollection<HistorialEstadoTransaccion> HistorialEstadoTransaccion { get; set; } = new List<HistorialEstadoTransaccion>();

    public virtual Usuario? IdAdminBloqueoNavigation { get; set; }

    public virtual EstadoCuenta IdEstadoCuentaNavigation { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Usuario> InverseIdAdminBloqueoNavigation { get; set; } = new List<Usuario>();

    public virtual ICollection<MensajeChat> MensajeChat { get; set; } = new List<MensajeChat>();

    public virtual ICollection<MetodoPago> MetodoPago { get; set; } = new List<MetodoPago>();

    public virtual ICollection<Notificacion> Notificacion { get; set; } = new List<Notificacion>();

    public virtual ICollection<Oferta> Oferta { get; set; } = new List<Oferta>();

    public virtual ICollection<TokenRecuperacion> TokenRecuperacion { get; set; } = new List<TokenRecuperacion>();

    public virtual ICollection<Transaccion> TransaccionIdUsuarioCompradorNavigation { get; set; } = new List<Transaccion>();

    public virtual ICollection<Transaccion> TransaccionIdUsuarioVendedorNavigation { get; set; } = new List<Transaccion>();

    public virtual ICollection<VerificacionIdentidad> VerificacionIdentidadIdAdminResolucionNavigation { get; set; } = new List<VerificacionIdentidad>();

    public virtual ICollection<VerificacionIdentidad> VerificacionIdentidadIdUsuarioNavigation { get; set; } = new List<VerificacionIdentidad>();
}
