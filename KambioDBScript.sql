-- ============================================================
-- BASE DE DATOS: KambioDB
-- Proyecto: Intercambio P2P de Divisas
-- Desarrollo de Aplicaciones Web - ESAN
-- ============================================================

CREATE DATABASE KambioDB;
GO

USE KambioDB;
GO

-- ============================================================
-- TABLAS DE CATÁLOGO / MAESTRAS
-- ============================================================

-- Roles del sistema (USU, ADM)
CREATE TABLE Rol (
    IdRol       INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      VARCHAR(50)  NOT NULL UNIQUE,   -- 'Usuario', 'Administrador'
    Descripcion VARCHAR(200) NULL
);

-- Estados posibles de una cuenta de usuario
CREATE TABLE EstadoCuenta (
    IdEstadoCuenta  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          VARCHAR(50) NOT NULL UNIQUE  -- 'Activo', 'Suspendido', 'Bloqueado'
);

-- Divisas soportadas por la plataforma
CREATE TABLE Divisa (
    IdDivisa    INT IDENTITY(1,1) PRIMARY KEY,
    Codigo      VARCHAR(10)  NOT NULL UNIQUE,   -- 'USD', 'PEN', 'EUR', 'GBP', 'JPY'...
    Nombre      VARCHAR(100) NOT NULL,
    Simbolo     VARCHAR(5)   NOT NULL
);

-- Bancos y billeteras digitales (BCP, Interbank, Yape, Plin, BBVA, Scotia...)
CREATE TABLE Banco (
    IdBanco     INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      VARCHAR(100) NOT NULL UNIQUE,
    Tipo        VARCHAR(50)  NOT NULL            -- 'Banco', 'Billetera Digital'
);

-- Estados de una oferta publicada
CREATE TABLE EstadoOferta (
    IdEstadoOferta  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          VARCHAR(50) NOT NULL UNIQUE  -- 'Activa', 'Cancelada', 'Completada', 'Emparejada'
);

-- Tipos de oferta
CREATE TABLE TipoOferta (
    IdTipoOferta    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          VARCHAR(20) NOT NULL UNIQUE  -- 'Compra', 'Venta'
);

-- Estados de una transacción
CREATE TABLE EstadoTransaccion (
    IdEstadoTransaccion INT IDENTITY(1,1) PRIMARY KEY,
    Nombre              VARCHAR(50) NOT NULL UNIQUE
    -- 'Pendiente', 'En Proceso', 'Pago Realizado', 'Completada', 'Cancelada', 'En Disputa'
);

-- Estados de una disputa
CREATE TABLE EstadoDisputa (
    IdEstadoDisputa INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          VARCHAR(50) NOT NULL UNIQUE  -- 'Abierta', 'Resuelta', 'Rechazada'
);

-- Estados de verificación de identidad
CREATE TABLE EstadoVerificacion (
    IdEstadoVerificacion    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre                  VARCHAR(50) NOT NULL UNIQUE  -- 'Pendiente', 'Aprobada', 'Rechazada'
);

-- Tipos de notificación
CREATE TABLE TipoNotificacion (
    IdTipoNotificacion  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre              VARCHAR(100) NOT NULL UNIQUE
    -- 'Oferta Aceptada', 'Cambio de Estado Transaccion', 'Match Encontrado',
    -- 'Voucher Subido', 'Alerta Tipo de Cambio', 'Disputa Reportada'
);

-- ============================================================
-- USUARIOS
-- ============================================================

-- US-001, US-002, US-015, US-019
CREATE TABLE Usuario (
    IdUsuario           INT IDENTITY(1,1) PRIMARY KEY,
    IdRol               INT          NOT NULL REFERENCES Rol(IdRol),
    IdEstadoCuenta      INT          NOT NULL REFERENCES EstadoCuenta(IdEstadoCuenta),
    Nombres             VARCHAR(100) NOT NULL,
    Apellidos           VARCHAR(100) NOT NULL,
    Correo              VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash        VARCHAR(256) NOT NULL,
    Telefono            VARCHAR(20)  NULL,
    FotoPerfil          VARCHAR(500) NULL,          -- ruta o URL de la imagen
    EsVerificado        BIT          NOT NULL DEFAULT 0,
    CalificacionPromedio DECIMAL(3,2) NOT NULL DEFAULT 0.00,
    TotalOrdenes        INT          NOT NULL DEFAULT 0,
    FechaRegistro       DATETIME     NOT NULL DEFAULT GETDATE(),
    FechaUltimaConexion DATETIME     NULL,
    -- Bloqueo/Suspensión (US-019)
    MotivoBloqueo       VARCHAR(500) NULL,
    FechaBloqueo        DATETIME     NULL,
    IdAdminBloqueo      INT          NULL REFERENCES Usuario(IdUsuario)
);

-- US-013: Token de recuperación de contraseña
CREATE TABLE TokenRecuperacion (
    IdToken         INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario       INT          NOT NULL REFERENCES Usuario(IdUsuario),
    Token           VARCHAR(256) NOT NULL UNIQUE,
    FechaExpiracion DATETIME     NOT NULL,           -- vigencia 30 minutos
    Usado           BIT          NOT NULL DEFAULT 0,
    FechaCreacion   DATETIME     NOT NULL DEFAULT GETDATE()
);

-- US-023: Verificación de identidad (DNI / documento)
CREATE TABLE VerificacionIdentidad (
    IdVerificacion          INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario               INT          NOT NULL REFERENCES Usuario(IdUsuario),
    IdEstadoVerificacion    INT          NOT NULL REFERENCES EstadoVerificacion(IdEstadoVerificacion),
    RutaImagen              VARCHAR(500) NOT NULL,
    FechaSolicitud          DATETIME     NOT NULL DEFAULT GETDATE(),
    FechaResolucion         DATETIME     NULL,
    IdAdminResolucion       INT          NULL REFERENCES Usuario(IdUsuario),
    ObservacionAdmin        VARCHAR(500) NULL
);

-- ============================================================
-- MÉTODOS DE PAGO (CUENTAS BANCARIAS) — US-018
-- ============================================================

CREATE TABLE MetodoPago (
    IdMetodoPago    INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario       INT          NOT NULL REFERENCES Usuario(IdUsuario),
    IdBanco         INT          NOT NULL REFERENCES Banco(IdBanco),
    TipoCuenta      VARCHAR(50)  NOT NULL,           -- 'Ahorros', 'Corriente', 'Yape', 'Plin'...
    NumeroCuenta    VARCHAR(30)  NOT NULL,
    CCI             VARCHAR(30)  NULL,
    Alias           VARCHAR(100) NULL,
    Activo          BIT          NOT NULL DEFAULT 1,
    FechaRegistro   DATETIME     NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- OFERTAS — US-005, US-006, US-016, US-021
-- ============================================================

CREATE TABLE Oferta (
    IdOferta            INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario           INT             NOT NULL REFERENCES Usuario(IdUsuario),
    IdTipoOferta        INT             NOT NULL REFERENCES TipoOferta(IdTipoOferta),
    IdEstadoOferta      INT             NOT NULL REFERENCES EstadoOferta(IdEstadoOferta),
    IdDivisaOrigen      INT             NOT NULL REFERENCES Divisa(IdDivisa),
    IdDivisaDestino     INT             NOT NULL REFERENCES Divisa(IdDivisa),
    MontoDisponible     DECIMAL(18,4)   NOT NULL,
    MontoMinimo         DECIMAL(18,4)   NOT NULL,    -- US-021
    MontoMaximo         DECIMAL(18,4)   NOT NULL,    -- US-021
    TasaCambio          DECIMAL(18,6)   NOT NULL,
    FechaPublicacion    DATETIME        NOT NULL DEFAULT GETDATE(),
    FechaCancelacion    DATETIME        NULL,        -- US-016
    FechaCompletado     DATETIME        NULL
);

-- Relación Oferta <-> MetodosPago aceptados (una oferta puede aceptar varios métodos)
CREATE TABLE OfertaMetodoPago (
    IdOfertaMetodoPago  INT IDENTITY(1,1) PRIMARY KEY,
    IdOferta            INT NOT NULL REFERENCES Oferta(IdOferta),
    IdBanco             INT NOT NULL REFERENCES Banco(IdBanco),
    CONSTRAINT UQ_OfertaMetodo UNIQUE (IdOferta, IdBanco)
);

-- ============================================================
-- MATCHING — US-017
-- ============================================================

CREATE TABLE MatchOferta (
    IdMatch         INT IDENTITY(1,1) PRIMARY KEY,
    IdOfertaOrigen  INT         NOT NULL REFERENCES Oferta(IdOferta),
    IdOfertaMatch   INT         NOT NULL REFERENCES Oferta(IdOferta),
    Estado          VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
    -- 'Pendiente', 'Aceptado', 'Rechazado'
    FechaMatch      DATETIME    NOT NULL DEFAULT GETDATE(),
    FechaRespuesta  DATETIME    NULL
);

-- ============================================================
-- TRANSACCIONES — US-007, US-009
-- ============================================================

CREATE TABLE Transaccion (
    IdTransaccion           INT IDENTITY(1,1) PRIMARY KEY,
    IdOferta                INT             NOT NULL REFERENCES Oferta(IdOferta),
    IdUsuarioComprador      INT             NOT NULL REFERENCES Usuario(IdUsuario),
    IdUsuarioVendedor       INT             NOT NULL REFERENCES Usuario(IdUsuario),
    IdEstadoTransaccion     INT             NOT NULL REFERENCES EstadoTransaccion(IdEstadoTransaccion),
    IdDivisaOrigen          INT             NOT NULL REFERENCES Divisa(IdDivisa),
    IdDivisaDestino         INT             NOT NULL REFERENCES Divisa(IdDivisa),
    Monto                   DECIMAL(18,4)   NOT NULL,
    MontoEquivalente        DECIMAL(18,4)   NOT NULL,
    TasaCambioAplicada      DECIMAL(18,6)   NOT NULL,
    TipoOperacion           VARCHAR(10)     NOT NULL,   -- 'Compra' o 'Venta'
    FechaInicio             DATETIME        NOT NULL DEFAULT GETDATE(),
    FechaConfirmacionPago   DATETIME        NULL,       -- US-007
    FechaCompletado         DATETIME        NULL,
    FechaCancelacion        DATETIME        NULL,
    -- Confirmaciones de ambas partes (US-007)
    ConfirmadoPorComprador  BIT             NOT NULL DEFAULT 0,
    ConfirmadoPorVendedor   BIT             NOT NULL DEFAULT 0
);

-- Historial de cambios de estado de una transacción (US-009)
CREATE TABLE HistorialEstadoTransaccion (
    IdHistorial             INT IDENTITY(1,1) PRIMARY KEY,
    IdTransaccion           INT         NOT NULL REFERENCES Transaccion(IdTransaccion),
    IdEstadoTransaccion     INT         NOT NULL REFERENCES EstadoTransaccion(IdEstadoTransaccion),
    FechaCambio             DATETIME    NOT NULL DEFAULT GETDATE(),
    Observacion             VARCHAR(500) NULL,
    IdUsuarioCambio         INT         NOT NULL REFERENCES Usuario(IdUsuario)
);

-- ============================================================
-- COMPROBANTES / VOUCHERS — US-010
-- ============================================================

CREATE TABLE Comprobante (
    IdComprobante   INT IDENTITY(1,1) PRIMARY KEY,
    IdTransaccion   INT          NOT NULL REFERENCES Transaccion(IdTransaccion),
    IdUsuario       INT          NOT NULL REFERENCES Usuario(IdUsuario),
    RutaImagen      VARCHAR(500) NOT NULL,           -- JPG/PNG
    FechaSubida     DATETIME     NOT NULL DEFAULT GETDATE(),
    Activo          BIT          NOT NULL DEFAULT 1
);

-- ============================================================
-- CALIFICACIONES — US-011
-- ============================================================

CREATE TABLE Calificacion (
    IdCalificacion      INT IDENTITY(1,1) PRIMARY KEY,
    IdTransaccion       INT          NOT NULL REFERENCES Transaccion(IdTransaccion),
    IdUsuarioEvalua     INT          NOT NULL REFERENCES Usuario(IdUsuario),
    IdUsuarioEvaluado   INT          NOT NULL REFERENCES Usuario(IdUsuario),
    Estrellas           TINYINT      NOT NULL CHECK (Estrellas BETWEEN 1 AND 5),
    Comentario          VARCHAR(500) NULL,
    FechaCalificacion   DATETIME     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Calificacion UNIQUE (IdTransaccion, IdUsuarioEvalua, IdUsuarioEvaluado)
);

-- ============================================================
-- DISPUTAS — US-008
-- ============================================================

CREATE TABLE Disputa (
    IdDisputa           INT IDENTITY(1,1) PRIMARY KEY,
    IdTransaccion       INT          NOT NULL REFERENCES Transaccion(IdTransaccion),
    IdUsuarioReporta    INT          NOT NULL REFERENCES Usuario(IdUsuario),
    IdEstadoDisputa     INT          NOT NULL REFERENCES EstadoDisputa(IdEstadoDisputa),
    Descripcion         VARCHAR(1000) NOT NULL,
    FechaReporte        DATETIME     NOT NULL DEFAULT GETDATE(),
    FechaResolucion     DATETIME     NULL,
    IdAdminResolucion   INT          NULL REFERENCES Usuario(IdUsuario),
    ResolucionDetalle   VARCHAR(1000) NULL
);

-- ============================================================
-- NOTIFICACIONES — US-014
-- ============================================================

CREATE TABLE Notificacion (
    IdNotificacion      INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario           INT          NOT NULL REFERENCES Usuario(IdUsuario),
    IdTipoNotificacion  INT          NOT NULL REFERENCES TipoNotificacion(IdTipoNotificacion),
    Titulo              VARCHAR(200) NOT NULL,
    Mensaje             VARCHAR(500) NOT NULL,
    Leida               BIT          NOT NULL DEFAULT 0,
    FechaCreacion       DATETIME     NOT NULL DEFAULT GETDATE(),
    FechaLectura        DATETIME     NULL,
    -- Referencia opcional al objeto relacionado
    IdReferencia        INT          NULL,    -- puede ser IdTransaccion, IdOferta, IdDisputa, etc.
    TipoReferencia      VARCHAR(50)  NULL     -- 'Transaccion', 'Oferta', 'Disputa', 'Match'
);

-- ============================================================
-- CHAT INTERNO — US-022
-- ============================================================

CREATE TABLE MensajeChat (
    IdMensaje       INT IDENTITY(1,1) PRIMARY KEY,
    IdTransaccion   INT           NOT NULL REFERENCES Transaccion(IdTransaccion),
    IdUsuarioEnvia  INT           NOT NULL REFERENCES Usuario(IdUsuario),
    Mensaje         VARCHAR(2000) NOT NULL,
    FechaEnvio      DATETIME      NOT NULL DEFAULT GETDATE(),
    Leido           BIT           NOT NULL DEFAULT 0
);

-- ============================================================
-- ALERTAS DE TIPO DE CAMBIO — US-024
-- ============================================================

CREATE TABLE AlertaTipoCambio (
    IdAlerta        INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario       INT             NOT NULL REFERENCES Usuario(IdUsuario),
    IdDivisaOrigen  INT             NOT NULL REFERENCES Divisa(IdDivisa),
    IdDivisaDestino INT             NOT NULL REFERENCES Divisa(IdDivisa),
    ValorUmbral     DECIMAL(18,6)   NOT NULL,
    Activa          BIT             NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME        NOT NULL DEFAULT GETDATE(),
    FechaDisparo    DATETIME        NULL    -- cuando se cumplió la condición
);

-- ============================================================
-- DATOS INICIALES (CATÁLOGOS)
-- ============================================================

INSERT INTO Rol (Nombre, Descripcion) VALUES
('Usuario',       'Usuario comprador o vendedor de divisas'),
('Administrador', 'Administrador del sistema con acceso al panel');

INSERT INTO EstadoCuenta (Nombre) VALUES
('Activo'), ('Suspendido'), ('Bloqueado');

INSERT INTO Divisa (Codigo, Nombre, Simbolo) VALUES
('USD', 'Dólar Estadounidense', '$'),
('PEN', 'Sol Peruano',          'S/'),
('EUR', 'Euro',                 '€'),
('GBP', 'Libra Esterlina',      '£'),
('JPY', 'Yen Japonés',          '¥'),
('CHF', 'Franco Suizo',         'Fr');

INSERT INTO Banco (Nombre, Tipo) VALUES
('BCP',         'Banco'),
('Interbank',   'Banco'),
('BBVA',        'Banco'),
('Scotiabank',  'Banco'),
('BanBif',      'Banco'),
('Yape',        'Billetera Digital'),
('Plin',        'Billetera Digital');

INSERT INTO EstadoOferta (Nombre) VALUES
('Activa'), ('Cancelada'), ('Completada'), ('Emparejada');

INSERT INTO TipoOferta (Nombre) VALUES
('Compra'), ('Venta');

INSERT INTO EstadoTransaccion (Nombre) VALUES
('Pendiente'), ('En Proceso'), ('Pago Realizado'),
('Completada'), ('Cancelada'), ('En Disputa');

INSERT INTO EstadoDisputa (Nombre) VALUES
('Abierta'), ('Resuelta'), ('Rechazada');

INSERT INTO EstadoVerificacion (Nombre) VALUES
('Pendiente'), ('Aprobada'), ('Rechazada');

INSERT INTO TipoNotificacion (Nombre) VALUES
('Oferta Aceptada'),
('Cambio de Estado Transaccion'),
('Match Encontrado'),
('Voucher Subido'),
('Alerta Tipo de Cambio'),
('Disputa Reportada'),
('Cuenta Bloqueada'),
('Verificacion de Identidad');
GO