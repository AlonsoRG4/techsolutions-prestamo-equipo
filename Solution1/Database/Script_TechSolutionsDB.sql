/* ============================================================================
   TECHSOLUTIONS S.A. - Sistema de Préstamo de Equipo Tecnológico
   Script de creación de base de datos - SQL Server
   ============================================================================
   Incluye:
     1) Tablas estándar de ASP.NET Identity 2.x (AspNetUsers, AspNetRoles,
        AspNetUserRoles, AspNetUserClaims, AspNetUserLogins).
     2) Tablas propias del sistema: Equipos, Empleados, Prestamos.
     3) Datos semilla: roles Administrador/Operador y un usuario administrador.

   Ejecutar completo en SQL Server Management Studio, o pegar en el editor de
   consultas del panel de tu hosting (Somee / MonsterASP.NET) apuntando a la
   base de datos ya creada para tu cuenta.

   Usuario semilla: admin@techsolutions.com   |   Contraseña: Admin123!
   ============================================================================ */

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'TechSolutionsDB')
BEGIN
    CREATE DATABASE TechSolutionsDB;
END
GO

USE TechSolutionsDB;
GO

/* ----------------------------------------------------------------------------
   Limpieza (si el script se vuelve a ejecutar sobre una BD existente)
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Prestamos', 'U') IS NOT NULL DROP TABLE dbo.Prestamos;
IF OBJECT_ID('dbo.Empleados', 'U') IS NOT NULL DROP TABLE dbo.Empleados;
IF OBJECT_ID('dbo.Equipos', 'U') IS NOT NULL DROP TABLE dbo.Equipos;
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;
GO

/* ============================================================================
   TABLAS DE ASP.NET IDENTITY (esquema estándar Microsoft.AspNet.Identity 2.x)
   ============================================================================ */

CREATE TABLE dbo.AspNetRoles
(
    Id      NVARCHAR(128) NOT NULL,
    Name    NVARCHAR(256) NOT NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY CLUSTERED (Id)
);
GO
CREATE UNIQUE INDEX RoleNameIndex ON dbo.AspNetRoles (Name);
GO

CREATE TABLE dbo.AspNetUsers
(
    Id                      NVARCHAR(128)   NOT NULL,
    Email                   NVARCHAR(256)   NULL,
    EmailConfirmed          BIT             NOT NULL DEFAULT (0),
    PasswordHash            NVARCHAR(MAX)   NULL,
    SecurityStamp           NVARCHAR(MAX)   NULL,
    PhoneNumber             NVARCHAR(MAX)   NULL,
    PhoneNumberConfirmed    BIT             NOT NULL DEFAULT (0),
    TwoFactorEnabled        BIT             NOT NULL DEFAULT (0),
    LockoutEndDateUtc       DATETIME        NULL,
    LockoutEnabled          BIT             NOT NULL DEFAULT (0),
    AccessFailedCount       INT             NOT NULL DEFAULT (0),
    UserName                NVARCHAR(256)   NOT NULL,
    NombreCompleto          NVARCHAR(200)   NULL,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY CLUSTERED (Id)
);
GO
CREATE UNIQUE INDEX UserNameIndex ON dbo.AspNetUsers (UserName);
GO

CREATE TABLE dbo.AspNetUserRoles
(
    UserId  NVARCHAR(128) NOT NULL,
    RoleId  NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY CLUSTERED (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_User FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Role FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles (Id) ON DELETE CASCADE
);
GO
CREATE INDEX IX_AspNetUserRoles_UserId ON dbo.AspNetUserRoles (UserId);
CREATE INDEX IX_AspNetUserRoles_RoleId ON dbo.AspNetUserRoles (RoleId);
GO

CREATE TABLE dbo.AspNetUserClaims
(
    Id          INT IDENTITY(1,1)   NOT NULL,
    UserId      NVARCHAR(128)       NOT NULL,
    ClaimType   NVARCHAR(MAX)       NULL,
    ClaimValue  NVARCHAR(MAX)       NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_AspNetUserClaims_User FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE
);
GO
CREATE INDEX IX_AspNetUserClaims_UserId ON dbo.AspNetUserClaims (UserId);
GO

CREATE TABLE dbo.AspNetUserLogins
(
    LoginProvider   NVARCHAR(128) NOT NULL,
    ProviderKey     NVARCHAR(128) NOT NULL,
    UserId          NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey, UserId),
    CONSTRAINT FK_AspNetUserLogins_User FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE
);
GO
CREATE INDEX IX_AspNetUserLogins_UserId ON dbo.AspNetUserLogins (UserId);
GO

/* ============================================================================
   TABLAS DEL SISTEMA
   ============================================================================ */

CREATE TABLE dbo.Equipos
(
    Id      INT IDENTITY(1,1)   NOT NULL,
    Nombre  NVARCHAR(100)       NOT NULL,
    Marca   NVARCHAR(80)        NOT NULL,
    Modelo  NVARCHAR(80)        NOT NULL,
    Serie   NVARCHAR(80)        NOT NULL,
    Estado  NVARCHAR(30)        NOT NULL DEFAULT ('Disponible'),
    CONSTRAINT PK_Equipos PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Equipos_Serie UNIQUE (Serie),
    CONSTRAINT CK_Equipos_Estado CHECK (Estado IN ('Disponible','Prestado','Mantenimiento'))
);
GO

CREATE TABLE dbo.Empleados
(
    Id              INT IDENTITY(1,1)   NOT NULL,
    Nombre          NVARCHAR(100)       NOT NULL,
    Departamento    NVARCHAR(80)        NOT NULL,
    Correo          NVARCHAR(150)       NOT NULL,
    Telefono        NVARCHAR(20)        NOT NULL,
    CONSTRAINT PK_Empleados PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Empleados_Correo UNIQUE (Correo)
);
GO

CREATE TABLE dbo.Prestamos
(
    Id              INT IDENTITY(1,1)   NOT NULL,
    EquipoId        INT                 NOT NULL,
    EmpleadoId      INT                 NOT NULL,
    FechaPrestamo   DATETIME            NOT NULL,
    FechaEntrega    DATETIME            NULL,
    Estatus         NVARCHAR(30)        NOT NULL DEFAULT ('Prestado'),
    CONSTRAINT PK_Prestamos PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Prestamos_Equipos FOREIGN KEY (EquipoId) REFERENCES dbo.Equipos (Id),
    CONSTRAINT FK_Prestamos_Empleados FOREIGN KEY (EmpleadoId) REFERENCES dbo.Empleados (Id),
    CONSTRAINT CK_Prestamos_Estatus CHECK (Estatus IN ('Prestado','Devuelto'))
);
GO

CREATE INDEX IX_Prestamos_EquipoId ON dbo.Prestamos (EquipoId);
CREATE INDEX IX_Prestamos_EmpleadoId ON dbo.Prestamos (EmpleadoId);
GO

/* ============================================================================
   DATOS SEMILLA
   ============================================================================ */

INSERT INTO dbo.AspNetRoles (Id, Name) VALUES
(N'ca505699-85c4-4fb8-8c45-aeb205e8b4ed', N'Administrador'),
(N'ced1cfdb-07c5-4e76-ab74-eb549bac69cf', N'Operador');
GO

-- Usuario administrador inicial.
-- Correo/Usuario: admin@techsolutions.com   |   Contraseña: Admin123!
-- El PasswordHash fue generado con el mismo algoritmo que usa
-- Microsoft.AspNet.Identity.Core 2.x (PBKDF2-HMACSHA1, 1000 iteraciones,
-- salt de 16 bytes, subkey de 32 bytes, con el byte de formato 0x00 al inicio).
INSERT INTO dbo.AspNetUsers
(Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, NombreCompleto)
VALUES
(N'd2ef63ed-c099-4922-81ce-ba47b3c8f6d1', N'admin@techsolutions.com', 1,
 N'AAdULheBak3Bsja0lfkqPD4RMA+7DaFWl2+IaOnhUlAi1UM1/iVBMMaEHAojN3pQmw==',
 N'041b66e6-8d6d-44fd-b9bd-ffdc62315f29',
 0, 0, 1, 0, N'admin@techsolutions.com', N'Administrador General');
GO

INSERT INTO dbo.AspNetUserRoles (UserId, RoleId) VALUES
(N'd2ef63ed-c099-4922-81ce-ba47b3c8f6d1', N'ca505699-85c4-4fb8-8c45-aeb205e8b4ed');
GO

-- Datos de ejemplo (opcional, cómodo para probar el CRUD de inmediato).
INSERT INTO dbo.Equipos (Nombre, Marca, Modelo, Serie, Estado) VALUES
(N'Laptop Dell Latitude', N'Dell', N'Latitude 5440', N'DL5440-0001', N'Disponible'),
(N'Laptop HP EliteBook', N'HP', N'EliteBook 840', N'HP840-0002', N'Disponible'),
(N'Proyector Epson', N'Epson', N'PowerLite X49', N'EPX49-0003', N'Disponible');
GO

INSERT INTO dbo.Empleados (Nombre, Departamento, Correo, Telefono) VALUES
(N'Juan Pérez', N'Sistemas', N'juan.perez@techsolutions.com', N'5512345678'),
(N'María López', N'Recursos Humanos', N'maria.lopez@techsolutions.com', N'5598765432');
GO
