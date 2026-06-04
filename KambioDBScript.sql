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






-- ============================================================
-- DATOS DE PRUEBA: US-001 al US-004
-- ============================================================

USE KambioDB;
GO

-- ============================================================
-- US-001: Registro de nuevo usuario
-- Simula usuarios ya registrados en la base de datos.
-- Passwords hasheadas con BCrypt para la contraseña "Password123"
-- ============================================================

INSERT INTO Usuario (IdRol, IdEstadoCuenta, Nombres, Apellidos, Correo, PasswordHash, Telefono, FotoPerfil, EsVerificado, CalificacionPromedio, TotalOrdenes, FechaRegistro)
VALUES
-- Usuario comprador activo
(1, 1, 'Juan', 'Diaz Torres',    'juan.diaz@gmail.com',    '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', '987654321', NULL, 0, 4.80, 145, DATEADD(DAY, -90, GETDATE())),
-- Usuaria vendedora activa
(1, 1, 'Maria', 'Elena Quispe',  'maria.elena@gmail.com',  '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', '912345678', NULL, 1, 4.97, 89,  DATEADD(DAY, -60, GETDATE())),
-- Usuario vendedor activo
(1, 1, 'Carlos', 'Tapia Mendoza','carlos.tapia@gmail.com', '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', '956789123', NULL, 1, 5.00, 522, DATEADD(DAY, -120, GETDATE())),
-- Usuario con cuenta SUSPENDIDA (para probar US-002 bloqueo)
(1, 2, 'Pedro', 'Rojas Llanos',  'pedro.rojas@gmail.com',  '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', '934567891', NULL, 0, 2.50, 10,  DATEADD(DAY, -30, GETDATE())),
-- Usuario con cuenta BLOQUEADA (para probar US-002 bloqueo)
(1, 3, 'Luis', 'Vargas Peña',    'luis.vargas@gmail.com',  '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', '945678912', NULL, 0, 1.00, 3,   DATEADD(DAY, -15, GETDATE())),
-- Administrador
(2, 1, 'Admin', 'Kambio',        'admin@kambio.com',       '$2a$11$KzQU7Wd1mH3sL9pXvN2oRuY8eC4tA6bI0jF5gM7nO1qP3rS9wZ2xV', NULL,        NULL, 1, 0.00, 0,   DATEADD(DAY, -180, GETDATE()));
GO

-- Registrar motivo de bloqueo para el usuario bloqueado (IdUsuario = 5)
UPDATE Usuario
SET MotivoBloqueo  = 'Fraude detectado en múltiples transacciones',
    FechaBloqueo   = DATEADD(DAY, -5, GETDATE()),
    IdAdminBloqueo = 6  -- el admin
WHERE IdUsuario = 5;
GO

-- ============================================================
-- US-002: Inicio de sesión
-- Token de recuperación de contraseña para probar US-013
-- (se incluye aquí porque el login lo referencia)
-- ============================================================

INSERT INTO TokenRecuperacion (IdUsuario, Token, FechaExpiracion, Usado)
VALUES
-- Token válido (aún no expirado) para juan.diaz
(1, 'TOKEN-VALIDO-ABC123DEF456GHI789', DATEADD(MINUTE, 25, GETDATE()), 0),
-- Token expirado (para probar el rechazo)
(2, 'TOKEN-EXPIRADO-XYZ987UVW654RST321', DATEADD(MINUTE, -60, GETDATE()), 0),
-- Token ya usado
(1, 'TOKEN-USADO-QWE111ASD222ZXC333', DATEADD(MINUTE, 20, GETDATE()), 1);
GO

-- ============================================================
-- MÉTODOS DE PAGO para los usuarios
-- (necesarios para que las ofertas tengan métodos asociados)
-- ============================================================

INSERT INTO MetodoPago (IdUsuario, IdBanco, TipoCuenta, NumeroCuenta, CCI, Alias)
VALUES
-- Juan: BCP y Yape
(1, 1, 'Ahorros',          '19512345678901', '00219500012345678901', 'Mi BCP'),
(1, 6, 'Billetera Digital', '987654321',      NULL,                   'Mi Yape'),
-- Maria: Interbank y Plin
(2, 2, 'Ahorros',          '20012345678',    '00320001012345678000', 'Interbank Maria'),
(2, 7, 'Billetera Digital', '912345678',      NULL,                   'Plin Maria'),
-- Carlos: BBVA y Scotiabank
(3, 3, 'Corriente',        '00110123456789', '01111001100123456789', 'BBVA Carlos'),
(3, 4, 'Ahorros',          '0009876543210',  '00900900009876543210', 'Scotia Carlos');
GO

-- ============================================================
-- US-003: Búsqueda y visualización de ofertas del Mercado P2P
-- Ofertas activas en distintos pares de divisas
-- ============================================================

-- Obtener IDs de referencia:
-- TipoOferta: 1=Compra, 2=Venta
-- EstadoOferta: 1=Activa
-- Divisa: 1=USD, 2=PEN, 3=EUR, 4=GBP, 5=JPY, 6=CHF

INSERT INTO Oferta (IdUsuario, IdTipoOferta, IdEstadoOferta, IdDivisaOrigen, IdDivisaDestino, MontoDisponible, MontoMinimo, MontoMaximo, TasaCambio, FechaPublicacion)
VALUES
-- Juan vende USD (los demás compran USD)  → aparece como "Comprar USD" para otros
(1, 2, 1, 1, 2, 1240.00, 100.00,  1240.00, 3.742, DATEADD(HOUR, -2,  GETDATE())),
-- Maria vende USD
(2, 2, 1, 1, 2,  500.00,  50.00,   500.00, 3.745, DATEADD(HOUR, -5,  GETDATE())),
-- Carlos vende USD (gran volumen)
(3, 2, 1, 1, 2, 4500.00, 500.00,  4500.00, 3.748, DATEADD(HOUR, -1,  GETDATE())),
-- Juan compra USD
(1, 1, 1, 2, 1,  800.00,  50.00,   800.00, 3.740, DATEADD(HOUR, -3,  GETDATE())),
-- Maria vende EUR
(2, 2, 1, 3, 2,  300.00,  50.00,   300.00, 4.120, DATEADD(HOUR, -8,  GETDATE())),
-- Carlos compra GBP
(3, 1, 1, 4, 2,  200.00, 100.00,   200.00, 4.850, DATEADD(HOUR, -12, GETDATE())),
-- Oferta CANCELADA (para probar US-016)
(2, 2, 2, 1, 2,  100.00,  20.00,   100.00, 3.730, DATEADD(DAY, -3,   GETDATE())),
-- Oferta COMPLETADA (para historial US-004)
(1, 2, 3, 1, 2, 1000.00, 100.00,  1000.00, 3.738, DATEADD(DAY, -10,  GETDATE()));
GO

-- Métodos de pago aceptados por cada oferta
-- Banco: 1=BCP, 2=Interbank, 3=BBVA, 4=Scotiabank, 6=Yape, 7=Plin

INSERT INTO OfertaMetodoPago (IdOferta, IdBanco)
VALUES
-- Oferta 1 (Juan): BCP e Interbank
(1, 1), (1, 2),
-- Oferta 2 (Maria): Yape y Plin
(2, 6), (2, 7),
-- Oferta 3 (Carlos): BBVA y Scotiabank
(3, 3), (3, 4),
-- Oferta 4 (Juan compra): BCP y Yape
(4, 1), (4, 6),
-- Oferta 5 (Maria EUR): Interbank
(5, 2),
-- Oferta 6 (Carlos GBP): BBVA
(6, 3),
-- Oferta 7 (cancelada): BCP
(7, 1),
-- Oferta 8 (completada): BCP e Interbank
(8, 1), (8, 2);
GO

-- ============================================================
-- US-004: Historial de transacciones
-- Transacciones en distintos estados y pares de divisas
-- ============================================================

INSERT INTO Transaccion (IdOferta, IdUsuarioComprador, IdUsuarioVendedor, IdEstadoTransaccion, IdDivisaOrigen, IdDivisaDestino, Monto, MontoEquivalente, TasaCambioAplicada, TipoOperacion, FechaInicio, FechaConfirmacionPago, FechaCompletado, ConfirmadoPorComprador, ConfirmadoPorVendedor)
VALUES
-- Transacción COMPLETADA: Juan compró USD a Carlos (USD/EUR)
(8, 1, 3, 4, 1, 3, 1200.00, 1128.40, 3.742, 'Compra', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -10, DATEADD(MINUTE, 10, GETDATE())), DATEADD(DAY, -10, DATEADD(MINUTE, 20, GETDATE())), 1, 1),
-- Transacción CANCELADA: Maria vendió GBP a Juan
(7, 1, 2, 5, 4, 2,  850.00, 1032.75, 4.850, 'Venta',  DATEADD(DAY, -8,  GETDATE()), NULL, NULL, 0, 0),
-- Transacción COMPLETADA: Juan compró USD a Carlos (USD/JPY)
(8, 1, 3, 4, 1, 5, 3500.00, 523250,  149.50, 'Compra', DATEADD(DAY, -6,  GETDATE()), DATEADD(DAY, -6, DATEADD(MINUTE, 15, GETDATE())), DATEADD(DAY, -6, DATEADD(MINUTE, 30, GETDATE())), 1, 1),
-- Transacción COMPLETADA: Juan compró EUR a Maria (EUR/CHF)
(5, 1, 2, 4, 3, 6,  500.00,  475.25, 4.120, 'Compra', DATEADD(DAY, -4,  GETDATE()), DATEADD(DAY, -4, DATEADD(MINUTE, 12, GETDATE())), DATEADD(DAY, -4, DATEADD(MINUTE, 25, GETDATE())), 1, 1),
-- Transacción EN PROCESO: Carlos compra USD a Maria
(2, 3, 2, 2, 1, 2,  300.00, 1123.50, 3.745, 'Compra', DATEADD(HOUR, -1, GETDATE()), NULL, NULL, 0, 0),
-- Transacción PENDIENTE: Juan compra USD a Carlos
(3, 1, 3, 1, 1, 2,  500.00, 1874.00, 3.748, 'Compra', DATEADD(MINUTE, -30, GETDATE()), NULL, NULL, 0, 0),
-- Transacción EN DISPUTA
(1, 3, 1, 6, 1, 2,  200.00,  748.40, 3.742, 'Compra', DATEADD(DAY, -2,  GETDATE()), DATEADD(DAY, -2, DATEADD(MINUTE, 10, GETDATE())), NULL, 1, 0);
GO

-- Historial de cambios de estado para trazabilidad (US-009)
INSERT INTO HistorialEstadoTransaccion (IdTransaccion, IdEstadoTransaccion, FechaCambio, Observacion, IdUsuarioCambio)
VALUES
-- Transacción 1: Pendiente → En Proceso → Pago Realizado → Completada
(1, 1, DATEADD(DAY, -10, GETDATE()),                              'Transacción iniciada',             1),
(1, 2, DATEADD(DAY, -10, DATEADD(MINUTE,  5, GETDATE())),         'Oferta aceptada por el vendedor',  3),
(1, 3, DATEADD(DAY, -10, DATEADD(MINUTE, 10, GETDATE())),         'Comprador confirmó el pago',       1),
(1, 4, DATEADD(DAY, -10, DATEADD(MINUTE, 20, GETDATE())),         'Vendedor confirmó recepción',      3),
-- Transacción 2: Pendiente → Cancelada
(2, 1, DATEADD(DAY, -8, GETDATE()),                               'Transacción iniciada',             1),
(2, 5, DATEADD(DAY, -8, DATEADD(MINUTE, 15, GETDATE())),          'Cancelada por inactividad',        2),
-- Transacción 3: Pendiente → En Proceso → Pago Realizado → Completada
(3, 1, DATEADD(DAY, -6, GETDATE()),                               'Transacción iniciada',             1),
(3, 2, DATEADD(DAY, -6, DATEADD(MINUTE,  8, GETDATE())),          'Oferta aceptada',                  3),
(3, 3, DATEADD(DAY, -6, DATEADD(MINUTE, 15, GETDATE())),          'Pago confirmado',                  1),
(3, 4, DATEADD(DAY, -6, DATEADD(MINUTE, 30, GETDATE())),          'Completada exitosamente',          3),
-- Transacción 5: Pendiente → En Proceso
(5, 1, DATEADD(HOUR, -1, GETDATE()),                              'Transacción iniciada',             3),
(5, 2, DATEADD(MINUTE, -45, GETDATE()),                           'En proceso',                       2),
-- Transacción 6: Pendiente
(6, 1, DATEADD(MINUTE, -30, GETDATE()),                           'Transacción iniciada',             1),
-- Transacción 7: Pendiente → En Proceso → En Disputa
(7, 1, DATEADD(DAY, -2, GETDATE()),                               'Transacción iniciada',             3),
(7, 2, DATEADD(DAY, -2, DATEADD(MINUTE, 5, GETDATE())),           'En proceso',                       1),
(7, 6, DATEADD(DAY, -2, DATEADD(MINUTE, 30, GETDATE())),          'Disputa abierta por el comprador', 3);
GO

-- Comprobantes de pago para transacciones completadas (US-010)
INSERT INTO Comprobante (IdTransaccion, IdUsuario, RutaImagen, FechaSubida)
VALUES
(1, 1, '/vouchers/transaccion_1_comprobante.jpg', DATEADD(DAY, -10, DATEADD(MINUTE, 10, GETDATE()))),
(3, 1, '/vouchers/transaccion_3_comprobante.png', DATEADD(DAY,  -6, DATEADD(MINUTE, 15, GETDATE()))),
(4, 1, '/vouchers/transaccion_4_comprobante.jpg', DATEADD(DAY,  -4, DATEADD(MINUTE, 12, GETDATE()))),
(7, 3, '/vouchers/transaccion_7_comprobante.png', DATEADD(DAY,  -2, DATEADD(MINUTE, 10, GETDATE())));
GO

-- Calificaciones de transacciones completadas (US-011)
INSERT INTO Calificacion (IdTransaccion, IdUsuarioEvalua, IdUsuarioEvaluado, Estrellas, Comentario, FechaCalificacion)
VALUES
(1, 1, 3, 5, 'Excelente vendedor, muy rápido y confiable.',          DATEADD(DAY, -9, GETDATE())),
(1, 3, 1, 5, 'Comprador serio, pagó de inmediato.',                  DATEADD(DAY, -9, GETDATE())),
(3, 1, 3, 5, 'Todo perfecto, muy buena tasa.',                       DATEADD(DAY, -5, GETDATE())),
(3, 3, 1, 4, 'Buen comprador, recomendado.',                         DATEADD(DAY, -5, GETDATE())),
(4, 1, 2, 5, 'Maria siempre cumple, muy confiable.',                 DATEADD(DAY, -3, GETDATE())),
(4, 2, 1, 5, 'Juan es un comprador excelente, sin problemas.',       DATEADD(DAY, -3, GETDATE()));
GO

-- Notificaciones de prueba (US-014)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, FechaCreacion, IdReferencia, TipoReferencia)
VALUES
(1, 2, 'Transacción completada',     'Tu transacción #1 ha sido completada exitosamente.',          1, DATEADD(DAY, -9,     GETDATE()), 1, 'Transaccion'),
(3, 1, 'Oferta aceptada',            'Tu oferta #3 fue aceptada por Juan Diaz.',                    1, DATEADD(DAY, -9,     GETDATE()), 3, 'Oferta'),
(1, 2, 'Transacción en proceso',     'Tu transacción #6 está en proceso.',                          0, DATEADD(MINUTE, -25, GETDATE()), 6, 'Transaccion'),
(3, 2, 'Nueva transacción pendiente','Carlos Tapia inició una transacción con tu oferta #2.',       0, DATEADD(HOUR, -1,    GETDATE()), 5, 'Transaccion'),
(1, 6, 'Disputa reportada',          'Se abrió una disputa en tu transacción #7. El admin revisará.',0,DATEADD(DAY, -2,     GETDATE()), 7, 'Transaccion');
GO

-- ============================================================
-- SCRIPTS DE PRUEBA - Emily Calderon Anaya (24100471)
-- US-013: Recuperación de contraseña
-- US-014: Notificaciones en tiempo real
-- US-015: Gestión de perfil de usuario
-- US-016: Cancelación de oferta publicada
-- Base de datos: KambioDB
-- ============================================================

USE KambioDB;
GO

-- ============================================================
-- DATOS BASE NECESARIOS (ejecutar primero si la BD está vacía)
-- Si ya existen estos usuarios, omitir esta sección
-- ============================================================

-- Usuario de prueba 1: Emily (usuario normal, activo)
-- Nota: PasswordHash debe generarse con bcrypt en el backend.
--       Aquí se coloca un hash de ejemplo para 'Password123'
INSERT INTO Usuario (IdRol, IdEstadoCuenta, Nombres, Apellidos, Correo, PasswordHash, Telefono)
VALUES (
    1,                          -- Rol: Usuario
    1,                          -- EstadoCuenta: Activo
    'Emily',
    'Calderon Anaya',
    'emily.calderon@kambio.pe',
    '$2a$10$EjemploHashBcryptAqui1234567890abcdef',  -- reemplazar con hash real
    '987654321'
);

-- Usuario de prueba 2: Carlos (para interactuar como contraparte)
INSERT INTO Usuario (IdRol, IdEstadoCuenta, Nombres, Apellidos, Correo, PasswordHash, Telefono)
VALUES (
    1,
    1,
    'Carlos',
    'Mendoza Rivera',
    'carlos.mendoza@kambio.pe',
    '$2a$10$EjemploHashBcryptAqui1234567890abcdef',
    '912345678'
);

-- Usuario de prueba 3: Admin (para pruebas de US-014 notificaciones admin)
INSERT INTO Usuario (IdRol, IdEstadoCuenta, Nombres, Apellidos, Correo, PasswordHash)
VALUES (
    2,                          -- Rol: Administrador
    1,
    'Admin',
    'Kambio',
    'admin@kambio.pe',
    '$2a$10$EjemploHashBcryptAqui1234567890abcdef'
);

GO

-- ============================================================
-- US-013: RECUPERACIÓN DE CONTRASEÑA
-- TokenRecuperacion: token con vigencia 30 minutos
-- ============================================================

-- Caso 1: Token válido (aún vigente, no usado)
INSERT INTO TokenRecuperacion (IdUsuario, Token, FechaExpiracion, Usado)
VALUES (
    1,                                              -- IdUsuario: Emily
    'tok_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8',   -- token simulado
    DATEADD(MINUTE, 30, GETDATE()),                 -- expira en 30 min
    0                                               -- no usado
);

-- Caso 2: Token ya expirado (para probar validación de expiración)
INSERT INTO TokenRecuperacion (IdUsuario, Token, FechaExpiracion, Usado)
VALUES (
    1,
    'tok_EXPIRADO_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx',
    DATEADD(MINUTE, -10, GETDATE()),                -- ya expiró hace 10 min
    0
);

-- Caso 3: Token ya usado (para probar que no se reutilice)
INSERT INTO TokenRecuperacion (IdUsuario, Token, FechaExpiracion, Usado)
VALUES (
    1,
    'tok_USADO_yyyyyyyyyyyyyyyyyyyyyyyyyyyyyy',
    DATEADD(MINUTE, 30, GETDATE()),
    1                                               -- ya fue usado
);

-- Verificación US-013: ver tokens del usuario
-- SELECT * FROM TokenRecuperacion WHERE IdUsuario = 1 ORDER BY FechaCreacion DESC;

GO

-- ============================================================
-- US-014: NOTIFICACIONES EN TIEMPO REAL
-- Se insertan notificaciones de los 4 tipos relevantes:
--   1. Oferta Aceptada
--   2. Cambio de Estado Transaccion
--   3. Match Encontrado
--   4. Voucher Subido
-- ============================================================

-- Notificación 1: Oferta aceptada (sin leer)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, IdReferencia, TipoReferencia)
VALUES (
    1,                              -- Emily recibe la notif
    1,                              -- TipoNotificacion: 'Oferta Aceptada'
    'Tu oferta fue aceptada',
    'Carlos Mendoza ha aceptado tu oferta de cambio de 1,000 USD. Procede con la transferencia.',
    0,                              -- No leída
    1,                              -- IdReferencia: IdOferta (ajustar al ID real)
    'Oferta'
);

-- Notificación 2: Cambio de estado - Transacción En Proceso (sin leer)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, IdReferencia, TipoReferencia)
VALUES (
    1,
    2,                              -- TipoNotificacion: 'Cambio de Estado Transaccion'
    'Transacción En Proceso',
    'Tu transacción #TRX-9844 ha pasado a estado En Proceso. Realiza la transferencia bancaria.',
    0,
    1,                              -- IdReferencia: IdTransaccion (ajustar al ID real)
    'Transaccion'
);

-- Notificación 3: Transacción completada (sin leer)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, IdReferencia, TipoReferencia)
VALUES (
    1,
    2,
    'Transacción Completada',
    'Los fondos de tu transacción #TRX-9821 han sido liberados exitosamente a tu cuenta bancaria.',
    0,
    2,
    'Transaccion'
);

-- Notificación 4: Transacción cancelada (ya leída — para mostrar estado leído)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, FechaLectura, IdReferencia, TipoReferencia)
VALUES (
    1,
    2,
    'Transacción Cancelada',
    'La oferta por 500 USD ha sido cancelada por falta de pago del contraparte.',
    1,                              -- Leída
    DATEADD(HOUR, -2, GETDATE()),   -- Se leyó hace 2 horas
    3,
    'Transaccion'
);

-- Notificación 5: Voucher subido por contraparte (sin leer)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, IdReferencia, TipoReferencia)
VALUES (
    1,
    4,                              -- TipoNotificacion: 'Voucher Subido'
    'Comprobante recibido',
    'Carlos Mendoza ha subido su comprobante de pago. Por favor verifica la transferencia.',
    0,
    1,
    'Transaccion'
);

-- Notificación 6: Match encontrado (sin leer)
INSERT INTO Notificacion (IdUsuario, IdTipoNotificacion, Titulo, Mensaje, Leida, IdReferencia, TipoReferencia)
VALUES (
    1,
    3,                              -- TipoNotificacion: 'Match Encontrado'
    'Match disponible',
    'Encontramos una coincidencia para tu oferta de 500 USD. Tasa propuesta: 3.742 PEN/USD.',
    0,
    1,
    'Match'
);

-- Verificación US-014: contar notificaciones no leídas de Emily
-- SELECT COUNT(*) AS NoLeidas FROM Notificacion WHERE IdUsuario = 1 AND Leida = 0;

-- Marcar todas como leídas (acción "Marcar todas como leídas")
-- UPDATE Notificacion SET Leida = 1, FechaLectura = GETDATE() WHERE IdUsuario = 1 AND Leida = 0;

GO

-- ============================================================
-- US-015: GESTIÓN DE PERFIL DE USUARIO
-- Actualización de nombre, teléfono y foto de perfil
-- ============================================================

-- Simular que Emily actualiza su perfil (nombre, teléfono, foto)
UPDATE Usuario
SET
    Nombres     = 'Emily Paola',
    Apellidos   = 'Calderon Anaya',
    Telefono    = '987654321',
    FotoPerfil  = '/uploads/perfiles/emily_calderon_foto.jpg'  -- ruta relativa al servidor
WHERE IdUsuario = 1;

-- Simular calificación promedio actualizada tras transacciones
UPDATE Usuario
SET
    CalificacionPromedio = 4.80,
    TotalOrdenes         = 12
WHERE IdUsuario = 1;

-- Verificación US-015: ver perfil completo de Emily
-- SELECT IdUsuario, Nombres, Apellidos, Correo, Telefono, FotoPerfil,
--        CalificacionPromedio, TotalOrdenes, EsVerificado
-- FROM Usuario WHERE IdUsuario = 1;

GO

-- ============================================================
-- US-016: CANCELACIÓN DE OFERTA PUBLICADA
-- Requiere datos de Divisa, Oferta y TipoOferta
-- ============================================================

-- Primero insertamos ofertas activas de Emily para luego cancelar una
-- (Las divisas USD=1, PEN=2 ya existen en los INSERTs del catálogo)

-- Oferta activa 1 de Emily (Compra USD/PEN) — esta se cancelará
INSERT INTO Oferta (IdUsuario, IdTipoOferta, IdEstadoOferta, IdDivisaOrigen, IdDivisaDestino,
                    MontoDisponible, MontoMinimo, MontoMaximo, TasaCambio)
VALUES (
    1,      -- Emily
    1,      -- TipoOferta: Compra
    1,      -- EstadoOferta: Activa
    1,      -- DivisaOrigen: USD
    2,      -- DivisaDestino: PEN
    1000.00,
    100.00,
    1000.00,
    3.742
);

-- Oferta activa 2 de Emily (Venta USD/PEN) — esta quedará activa
INSERT INTO Oferta (IdUsuario, IdTipoOferta, IdEstadoOferta, IdDivisaOrigen, IdDivisaDestino,
                    MontoDisponible, MontoMinimo, MontoMaximo, TasaCambio)
VALUES (
    1,
    2,      -- TipoOferta: Venta
    1,      -- EstadoOferta: Activa
    1,
    2,
    500.00,
    50.00,
    500.00,
    3.750
);

-- Oferta de otro usuario (Carlos) — para validar que Emily no cancele ofertas ajenas
INSERT INTO Oferta (IdUsuario, IdTipoOferta, IdEstadoOferta, IdDivisaOrigen, IdDivisaDestino,
                    MontoDisponible, MontoMinimo, MontoMaximo, TasaCambio)
VALUES (
    2,      -- Carlos
    1,
    1,
    1,
    2,
    2000.00,
    200.00,
    2000.00,
    3.748
);

-- Simular cancelación de la Oferta 1 de Emily (acción del usuario)
-- En el backend esto se haría mediante un UPDATE al presionar "Cancelar"
UPDATE Oferta
SET
    IdEstadoOferta  = 2,                        -- EstadoOferta: Cancelada
    FechaCancelacion = GETDATE()
WHERE
    IdOferta  = 1                               -- Ajustar al ID real generado
    AND IdUsuario = 1                           -- Solo el dueño puede cancelarla
    AND IdEstadoOferta = 1;                     -- Solo si está Activa

-- Verificación US-016: ver ofertas de Emily con su estado
-- SELECT o.IdOferta, to2.Nombre AS Tipo, eo.Nombre AS Estado,
--        o.MontoDisponible, o.TasaCambio, o.FechaPublicacion, o.FechaCancelacion
-- FROM Oferta o
-- JOIN TipoOferta to2 ON o.IdTipoOferta = to2.IdTipoOferta
-- JOIN EstadoOferta eo ON o.IdEstadoOferta = eo.IdEstadoOferta
-- WHERE o.IdUsuario = 1;

GO

-- ============================================================
-- CONSULTAS DE VERIFICACIÓN GENERAL (descomentar para probar)
-- ============================================================

-- Ver todos los tokens de recuperación
-- SELECT t.IdToken, u.Correo, t.Token, t.FechaExpiracion, t.Usado
-- FROM TokenRecuperacion t
-- JOIN Usuario u ON t.IdUsuario = u.IdUsuario
-- ORDER BY t.FechaCreacion DESC;

-- Ver notificaciones no leídas con tipo
-- SELECT n.IdNotificacion, tn.Nombre AS Tipo, n.Titulo, n.Mensaje,
--        n.Leida, n.FechaCreacion
-- FROM Notificacion n
-- JOIN TipoNotificacion tn ON n.IdTipoNotificacion = tn.IdTipoNotificacion
-- WHERE n.IdUsuario = 1
-- ORDER BY n.FechaCreacion DESC;

-- Ver perfil actualizado de Emily
-- SELECT Nombres, Apellidos, Correo, Telefono, FotoPerfil,
--        CalificacionPromedio, TotalOrdenes, EsVerificado
-- FROM Usuario WHERE IdUsuario = 1;

-- Ver estado de ofertas tras cancelación
-- SELECT o.IdOferta, to2.Nombre AS Tipo, eo.Nombre AS Estado,
--        o.MontoDisponible, o.FechaPublicacion, o.FechaCancelacion
-- FROM Oferta o
-- JOIN TipoOferta to2 ON o.IdTipoOferta = to2.IdTipoOferta
-- JOIN EstadoOferta eo ON o.IdEstadoOferta = eo.IdEstadoOferta
-- WHERE o.IdUsuario = 1;
