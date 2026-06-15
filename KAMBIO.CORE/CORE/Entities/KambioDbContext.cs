using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Core.Entities;

public partial class KambioDbContext : DbContext
{
    public KambioDbContext()
    {
    }

    public KambioDbContext(DbContextOptions<KambioDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertaTipoCambio> AlertaTipoCambio { get; set; }

    public virtual DbSet<Banco> Banco { get; set; }

    public virtual DbSet<Calificacion> Calificacion { get; set; }

    public virtual DbSet<Comprobante> Comprobante { get; set; }

    public virtual DbSet<Disputa> Disputa { get; set; }

    public virtual DbSet<Divisa> Divisa { get; set; }

    public virtual DbSet<EstadoCuenta> EstadoCuenta { get; set; }

    public virtual DbSet<EstadoDisputa> EstadoDisputa { get; set; }

    public virtual DbSet<EstadoOferta> EstadoOferta { get; set; }

    public virtual DbSet<EstadoTransaccion> EstadoTransaccion { get; set; }

    public virtual DbSet<EstadoVerificacion> EstadoVerificacion { get; set; }

    public virtual DbSet<HistorialEstadoTransaccion> HistorialEstadoTransaccion { get; set; }

    public virtual DbSet<MatchOferta> MatchOferta { get; set; }

    public virtual DbSet<MensajeChat> MensajeChat { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Notificacion> Notificacion { get; set; }

    public virtual DbSet<Oferta> Oferta { get; set; }

    public virtual DbSet<OfertaMetodoPago> OfertaMetodoPago { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<TipoNotificacion> TipoNotificacion { get; set; }

    public virtual DbSet<TipoOferta> TipoOferta { get; set; }

    public virtual DbSet<TokenRecuperacion> TokenRecuperacion { get; set; }

    public virtual DbSet<Transaccion> Transaccion { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<VerificacionIdentidad> VerificacionIdentidad { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=KambioDB;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertaTipoCambio>(entity =>
        {
            entity.HasKey(e => e.IdAlerta).HasName("PK__AlertaTi__D2CDBC4F3F095210");

            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaDisparo).HasColumnType("datetime");
            entity.Property(e => e.ValorUmbral).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.IdDivisaDestinoNavigation).WithMany(p => p.AlertaTipoCambioIdDivisaDestinoNavigation)
                .HasForeignKey(d => d.IdDivisaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AlertaTip__IdDiv__32AB8735");

            entity.HasOne(d => d.IdDivisaOrigenNavigation).WithMany(p => p.AlertaTipoCambioIdDivisaOrigenNavigation)
                .HasForeignKey(d => d.IdDivisaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AlertaTip__IdDiv__31B762FC");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.AlertaTipoCambio)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AlertaTip__IdUsu__30C33EC3");
        });

        modelBuilder.Entity<Banco>(entity =>
        {
            entity.HasKey(e => e.IdBanco).HasName("PK__Banco__2D3F553E8CB8434C");

            entity.HasIndex(e => e.Nombre, "UQ__Banco__75E3EFCFC075C8C5").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.HasKey(e => e.IdCalificacion).HasName("PK__Califica__40E4A751CA9128FD");

            entity.HasIndex(e => new { e.IdTransaccion, e.IdUsuarioEvalua, e.IdUsuarioEvaluado }, "UQ_Calificacion").IsUnique();

            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCalificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.Calificacion)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Calificac__IdTra__17F790F9");

            entity.HasOne(d => d.IdUsuarioEvaluaNavigation).WithMany(p => p.CalificacionIdUsuarioEvaluaNavigation)
                .HasForeignKey(d => d.IdUsuarioEvalua)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Calificac__IdUsu__18EBB532");

            entity.HasOne(d => d.IdUsuarioEvaluadoNavigation).WithMany(p => p.CalificacionIdUsuarioEvaluadoNavigation)
                .HasForeignKey(d => d.IdUsuarioEvaluado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Calificac__IdUsu__19DFD96B");
        });

        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.HasKey(e => e.IdComprobante).HasName("PK__Comproba__BF4686EDC67AF4D5");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaSubida)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RutaImagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.Comprobante)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comproban__IdTra__114A936A");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Comprobante)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comproban__IdUsu__123EB7A3");
        });

        modelBuilder.Entity<Disputa>(entity =>
        {
            entity.HasKey(e => e.IdDisputa).HasName("PK__Disputa__58C8CAE458CBCD99");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.FechaReporte)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaResolucion).HasColumnType("datetime");
            entity.Property(e => e.ResolucionDetalle)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdminResolucionNavigation).WithMany(p => p.DisputaIdAdminResolucionNavigation)
                .HasForeignKey(d => d.IdAdminResolucion)
                .HasConstraintName("FK__Disputa__IdAdmin__22751F6C");

            entity.HasOne(d => d.IdEstadoDisputaNavigation).WithMany(p => p.Disputa)
                .HasForeignKey(d => d.IdEstadoDisputa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disputa__IdEstad__208CD6FA");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.Disputa)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disputa__IdTrans__1EA48E88");

            entity.HasOne(d => d.IdUsuarioReportaNavigation).WithMany(p => p.DisputaIdUsuarioReportaNavigation)
                .HasForeignKey(d => d.IdUsuarioReporta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disputa__IdUsuar__1F98B2C1");
        });

        modelBuilder.Entity<Divisa>(entity =>
        {
            entity.HasKey(e => e.IdDivisa).HasName("PK__Divisa__DA960DCB18658465");

            entity.HasIndex(e => e.Codigo, "UQ__Divisa__06370DAC47B60564").IsUnique();

            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Simbolo)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoCuenta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCuenta).HasName("PK__EstadoCu__79FBA94674A57C47");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoCu__75E3EFCF1B10D259").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoDisputa>(entity =>
        {
            entity.HasKey(e => e.IdEstadoDisputa).HasName("PK__EstadoDi__7B3CD9CF7B0A64CD");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoDi__75E3EFCF8E050F95").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoOferta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoOferta).HasName("PK__EstadoOf__EEC64D187ED05556");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoOf__75E3EFCF95814949").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoTransaccion>(entity =>
        {
            entity.HasKey(e => e.IdEstadoTransaccion).HasName("PK__EstadoTr__BA2DF9B4083F9C45");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoTr__75E3EFCF27AC344D").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoVerificacion>(entity =>
        {
            entity.HasKey(e => e.IdEstadoVerificacion).HasName("PK__EstadoVe__B640AA301FBEE45E");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoVe__75E3EFCF3DCA9540").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialEstadoTransaccion>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__Historia__9CC7DBB4C478136B");

            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Observacion)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEstadoTransaccionNavigation).WithMany(p => p.HistorialEstadoTransaccion)
                .HasForeignKey(d => d.IdEstadoTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__IdEst__0C85DE4D");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.HistorialEstadoTransaccion)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__IdTra__0B91BA14");

            entity.HasOne(d => d.IdUsuarioCambioNavigation).WithMany(p => p.HistorialEstadoTransaccion)
                .HasForeignKey(d => d.IdUsuarioCambio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__IdUsu__0E6E26BF");
        });

        modelBuilder.Entity<MatchOferta>(entity =>
        {
            entity.HasKey(e => e.IdMatch).HasName("PK__MatchOfe__5CC9057C8001D858");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaMatch)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaRespuesta).HasColumnType("datetime");

            entity.HasOne(d => d.IdOfertaMatchNavigation).WithMany(p => p.MatchOfertaIdOfertaMatchNavigation)
                .HasForeignKey(d => d.IdOfertaMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchOfer__IdOfe__7C4F7684");

            entity.HasOne(d => d.IdOfertaOrigenNavigation).WithMany(p => p.MatchOfertaIdOfertaOrigenNavigation)
                .HasForeignKey(d => d.IdOfertaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MatchOfer__IdOfe__7B5B524B");
        });

        modelBuilder.Entity<MensajeChat>(entity =>
        {
            entity.HasKey(e => e.IdMensaje).HasName("PK__MensajeC__E4D2A47F44DE9F48");

            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(2000)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.MensajeChat)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MensajeCh__IdTra__2B0A656D");

            entity.HasOne(d => d.IdUsuarioEnviaNavigation).WithMany(p => p.MensajeChat)
                .HasForeignKey(d => d.IdUsuarioEnvia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MensajeCh__IdUsu__2BFE89A6");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__6F49A9BE66BB88A2");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Alias)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cci)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CCI");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.MetodoPago)
                .HasForeignKey(d => d.IdBanco)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MetodoPag__IdBan__6A30C649");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.MetodoPago)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MetodoPag__IdUsu__693CA210");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__F6CA0A8531A6C915");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaLectura).HasColumnType("datetime");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TipoReferencia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoNotificacionNavigation).WithMany(p => p.Notificacion)
                .HasForeignKey(d => d.IdTipoNotificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__IdTip__2645B050");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__IdUsu__25518C17");
        });

        modelBuilder.Entity<Oferta>(entity =>
        {
            entity.HasKey(e => e.IdOferta).HasName("PK__Oferta__5420E1DAFA501E05");

            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCompletado).HasColumnType("datetime");
            entity.Property(e => e.FechaPublicacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MontoDisponible).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MontoMaximo).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MontoMinimo).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TasaCambio).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.IdDivisaDestinoNavigation).WithMany(p => p.OfertaIdDivisaDestinoNavigation)
                .HasForeignKey(d => d.IdDivisaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Oferta__IdDivisa__72C60C4A");

            entity.HasOne(d => d.IdDivisaOrigenNavigation).WithMany(p => p.OfertaIdDivisaOrigenNavigation)
                .HasForeignKey(d => d.IdDivisaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Oferta__IdDivisa__71D1E811");

            entity.HasOne(d => d.IdEstadoOfertaNavigation).WithMany(p => p.Oferta)
                .HasForeignKey(d => d.IdEstadoOferta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Oferta__IdEstado__70DDC3D8");

            entity.HasOne(d => d.IdTipoOfertaNavigation).WithMany(p => p.Oferta)
                .HasForeignKey(d => d.IdTipoOferta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Oferta__IdTipoOf__6FE99F9F");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Oferta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Oferta__IdUsuari__6EF57B66");
        });

        modelBuilder.Entity<OfertaMetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdOfertaMetodoPago).HasName("PK__OfertaMe__D70ED21E9DE8BBDF");

            entity.HasIndex(e => new { e.IdOferta, e.IdBanco }, "UQ_OfertaMetodo").IsUnique();

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.OfertaMetodoPago)
                .HasForeignKey(d => d.IdBanco)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OfertaMet__IdBan__787EE5A0");

            entity.HasOne(d => d.IdOfertaNavigation).WithMany(p => p.OfertaMetodoPago)
                .HasForeignKey(d => d.IdOferta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OfertaMet__IdOfe__778AC167");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C03F4CA38");

            entity.HasIndex(e => e.Nombre, "UQ__Rol__75E3EFCF5EDF2876").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoNotificacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoNotificacion).HasName("PK__TipoNoti__0ECE0435533ED8FC");

            entity.HasIndex(e => e.Nombre, "UQ__TipoNoti__75E3EFCF74F9EDC7").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoOferta>(entity =>
        {
            entity.HasKey(e => e.IdTipoOferta).HasName("PK__TipoOfer__1C8263A23219A0AB");

            entity.HasIndex(e => e.Nombre, "UQ__TipoOfer__75E3EFCFA8435F81").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TokenRecuperacion>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PK__TokenRec__D6332447B7C1267E");

            entity.HasIndex(e => e.Token, "UQ__TokenRec__1EB4F8172C8F2644").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.Token)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TokenRecuperacion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TokenRecu__IdUsu__5EBF139D");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("PK__Transacc__334B1F7792507225");

            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCompletado).HasColumnType("datetime");
            entity.Property(e => e.FechaConfirmacionPago).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MontoEquivalente).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TasaCambioAplicada).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.TipoOperacion)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDivisaDestinoNavigation).WithMany(p => p.TransaccionIdDivisaDestinoNavigation)
                .HasForeignKey(d => d.IdDivisaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdDiv__05D8E0BE");

            entity.HasOne(d => d.IdDivisaOrigenNavigation).WithMany(p => p.TransaccionIdDivisaOrigenNavigation)
                .HasForeignKey(d => d.IdDivisaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdDiv__04E4BC85");

            entity.HasOne(d => d.IdEstadoTransaccionNavigation).WithMany(p => p.Transaccion)
                .HasForeignKey(d => d.IdEstadoTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdEst__03F0984C");

            entity.HasOne(d => d.IdOfertaNavigation).WithMany(p => p.Transaccion)
                .HasForeignKey(d => d.IdOferta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdOfe__01142BA1");

            entity.HasOne(d => d.IdUsuarioCompradorNavigation).WithMany(p => p.TransaccionIdUsuarioCompradorNavigation)
                .HasForeignKey(d => d.IdUsuarioComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdUsu__02084FDA");

            entity.HasOne(d => d.IdUsuarioVendedorNavigation).WithMany(p => p.TransaccionIdUsuarioVendedorNavigation)
                .HasForeignKey(d => d.IdUsuarioVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__IdUsu__02FC7413");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97C899CB13");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A19718FB72B").IsUnique();

            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CalificacionPromedio).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaBloqueo).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaUltimaConexion).HasColumnType("datetime");
            entity.Property(e => e.FotoPerfil)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MotivoBloqueo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdminBloqueoNavigation).WithMany(p => p.InverseIdAdminBloqueoNavigation)
                .HasForeignKey(d => d.IdAdminBloqueo)
                .HasConstraintName("FK__Usuario__IdAdmin__5AEE82B9");

            entity.HasOne(d => d.IdEstadoCuentaNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdEstadoCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__IdEstad__5629CD9C");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__IdRol__5535A963");
        });

        modelBuilder.Entity<VerificacionIdentidad>(entity =>
        {
            entity.HasKey(e => e.IdVerificacion).HasName("PK__Verifica__FB60903484EAE286");

            entity.Property(e => e.FechaResolucion).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ObservacionAdmin)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RutaImagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdminResolucionNavigation).WithMany(p => p.VerificacionIdentidadIdAdminResolucionNavigation)
                .HasForeignKey(d => d.IdAdminResolucion)
                .HasConstraintName("FK__Verificac__IdAdm__66603565");

            entity.HasOne(d => d.IdEstadoVerificacionNavigation).WithMany(p => p.VerificacionIdentidad)
                .HasForeignKey(d => d.IdEstadoVerificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Verificac__IdEst__6477ECF3");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.VerificacionIdentidadIdUsuarioNavigation)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Verificac__IdUsu__6383C8BA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
