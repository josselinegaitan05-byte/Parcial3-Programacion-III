SET NOCOUNT ON;
PRINT '--- Tabla Usuarios (contrasena guardada como hash BCrypt) ---';
SELECT Id, Usuario, NombreCompleto, PasswordHash FROM dbo.Usuarios;
PRINT '';
PRINT '--- Tabla IntentosLogin (intentos exitosos y fallidos) ---';
SELECT Id, Usuario, CASE WHEN Exitoso = 1 THEN 'EXITOSO' ELSE 'FALLIDO' END AS Resultado,
       CONVERT(VARCHAR(19), Fecha, 120) AS Fecha, Detalle
FROM dbo.IntentosLogin ORDER BY Id;
