-- Parcial #3 - Parte 3: Base de datos para Login con BCrypt
-- Ejecutar en SQL Server / LocalDB:
--   sqlcmd -S "(localdb)\MSSQLLocalDB" -i CrearBaseDatos.sql

IF DB_ID('LoginBCryptDB') IS NULL
    CREATE DATABASE LoginBCryptDB;
GO

USE LoginBCryptDB;
GO

-- Tabla de usuarios: la contraseña se guarda SOLO como hash BCrypt (nunca en texto plano)
IF OBJECT_ID('dbo.Usuarios') IS NULL
CREATE TABLE dbo.Usuarios (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Usuario         NVARCHAR(50)  NOT NULL UNIQUE,
    NombreCompleto  NVARCHAR(100) NOT NULL,
    PasswordHash    NVARCHAR(100) NOT NULL,   -- hash BCrypt (60 caracteres)
    FechaCreacion   DATETIME2     NOT NULL DEFAULT SYSDATETIME()
);
GO

-- Tabla de intentos de acceso (exitosos y fallidos) para análisis posterior
IF OBJECT_ID('dbo.IntentosLogin') IS NULL
CREATE TABLE dbo.IntentosLogin (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Usuario     NVARCHAR(50) NOT NULL,        -- usuario escrito (exista o no)
    Exitoso     BIT          NOT NULL,        -- 1 = éxito, 0 = fallido
    Fecha       DATETIME2    NOT NULL DEFAULT SYSDATETIME(),
    Detalle     NVARCHAR(100) NULL
);
GO

-- Consultas de ejemplo para el análisis:
-- SELECT Usuario, SUM(CASE WHEN Exitoso=1 THEN 1 ELSE 0 END) AS Exitosos,
--        SUM(CASE WHEN Exitoso=0 THEN 1 ELSE 0 END) AS Fallidos
-- FROM dbo.IntentosLogin GROUP BY Usuario;
