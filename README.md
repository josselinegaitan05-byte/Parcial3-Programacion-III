# Parcial #3 – Herramientas de la Programación Aplicada III (.NET)

**Universidad Tecnológica de Panamá** · Facultad de Ingeniería en Sistemas · Campus Víctor Levis Sasso
**Grupo:** 1IL133 · **Instructora:** Ing. Irina Fong · **II Semestre 2026**
**Estudiante:** _[Tu nombre completo]_ · **Fecha:** 09 de noviembre de 2026

---

## Contenido

1. [Descripción general](#1-descripción-general)
2. [Estructura del repositorio](#2-estructura-del-repositorio)
3. [Requisitos](#3-requisitos)
4. [Parte 1 – Clase abstracta y polimorfismo](#4-parte-1--clase-abstracta-y-polimorfismo-15-pts)
5. [Parte 2 – JSON con Newtonsoft.Json](#5-parte-2--json-con-newtonsoftjson-15-pts)
6. [Parte 3 – Login con BCrypt y SQL Server](#6-parte-3--login-con-bcrypt-y-sql-server-20-pts)
7. [Cómo ejecutar el proyecto](#7-cómo-ejecutar-el-proyecto)
8. [Resumen de cumplimiento](#8-resumen-de-cumplimiento)

---

## 1. Descripción general

Solución `Parcial3.slnx` con **tres proyectos de consola en .NET 10** que resuelven las tres partes del parcial:

| Proyecto | Tema | Tecnología |
|---|---|---|
| `Parte1_Figuras` | Jerarquía de clases, arreglo `Figura[]` y recorrido polimórfico | C# (POO) |
| `Parte2_Json` | Serialización de un objeto a JSON | Newtonsoft.Json 13.0.4 |
| `Parte3_LoginBCrypt` | Login con hash de contraseñas e historial de intentos | BCrypt.Net-Next 4.2.0 · SQL Server (LocalDB) |

---

## 2. Estructura del repositorio

```
Parcial Jossy/
├── Parcial3.slnx
├── README.md
├── .gitignore
├── Parte Práctica del Parcial #3.pdf      ← enunciado
├── Parte1_Figuras/
│   ├── Parte1_Figuras.csproj
│   └── Program.cs
├── Parte2_Json/
│   ├── Parte2_Json.csproj
│   └── Program.cs
├── Parte3_LoginBCrypt/
│   ├── Parte3_LoginBCrypt.csproj
│   └── Program.cs
├── BaseDatos/
│   ├── CrearBaseDatos.sql                 ← script de creación de la BD y tablas
│   ├── ConsultarDatos.sql                 ← consulta del contenido de las tablas
│   └── LoginBCryptDB.bak                  ← backup de la base de datos
└── capturas/                              ← evidencias de funcionamiento
    ├── 01_figuras.png
    ├── 02_json.png
    ├── 03_registro.png
    ├── 04_login_fallido.png
    ├── 05_login_exitoso.png
    ├── 06_historial.png
    └── 07_base_datos.png
```

---

## 3. Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server o **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`) – solo para la Parte 3
- `sqlcmd` o SQL Server Management Studio (SSMS) para crear/restaurar la base de datos
- Paquetes NuGet (se restauran solos con `dotnet run`):
  - `Newtonsoft.Json` 13.0.4
  - `BCrypt.Net-Next` 4.2.0
  - `Microsoft.Data.SqlClient` 7.1.1

---

## 4. Parte 1 – Clase abstracta y polimorfismo (15 pts)

**Enunciado:** diseñar una jerarquía con la clase abstracta `Figura` (métodos abstractos `CalcularArea()` y `CalcularPerimetro()`), implementar `Rectangulo`, `Cuadrado` y `Triangulo`, declarar un arreglo de tipo `Figura` y recorrerlo aplicando polimorfismo.

### Elementos clave y dónde están en el código

| # | Elemento | Puntos | Implementación |
|---|---|:---:|---|
| 1 | Clase abstracta (molde obligatorio) | 3 | `public abstract class Figura` |
| 2 | Métodos abstractos desarrollados en los hijos | 2 | `override CalcularArea()` y `override CalcularPerimetro()` en `Rectangulo`, `Cuadrado` y `Triangulo` |
| 3 | Arreglo unidimensional de tipo padre | 5 | `Figura[] figuras = new Figura[] { ... }` |
| 4 | Recorrido polimórfico | 5 | `for` que usa `Figura f = figuras[i]` y llama `f.CalcularArea()` / `f.CalcularPerimetro()` |

### Fragmentos principales

```csharp
public abstract class Figura
{
    public abstract double CalcularArea();
    public abstract double CalcularPerimetro();
    public string ObtenerNombre() => GetType().Name;
}

Figura[] figuras = new Figura[]
{
    new Rectangulo(5, 3),
    new Cuadrado(4),
    new Triangulo(3, 4, 5),
    new Rectangulo(10, 2.5),
    new Cuadrado(7.5),
    new Triangulo(6, 6, 6)
};

for (int i = 0; i < figuras.Length; i++)
{
    Figura f = figuras[i];   // variable del padre, objeto hijo
    Console.WriteLine($"{f.ObtenerNombre()} {f.CalcularArea():F2} {f.CalcularPerimetro():F2}");
}
```

- El `Triangulo` valida la **desigualdad triangular** y calcula el área con la **fórmula de Herón**.

### Evidencia

Cada objeto ejecuta su propia versión de `CalcularArea()` y `CalcularPerimetro()` aunque se acceda a él desde una variable de tipo `Figura`.

![Parte 1 – Figuras y polimorfismo](capturas/01_figuras.png)

---

## 5. Parte 2 – JSON con Newtonsoft.Json (15 pts)

**Enunciado:** ejecutar el script proporcionado que serializa un objeto a JSON.

### Pasos

1. Instalar el paquete: `dotnet add package Newtonsoft.Json`
2. Importar: `using Newtonsoft.Json;`
3. Crear un objeto con datos.
4. Convertirlo a cadena JSON con `JsonConvert.SerializeObject()`.

```csharp
using Newtonsoft.Json;

var persona = new
{
    Nombre = "Cristell",
    Edad = 20,
    Carrera = "Ingenieria en Sistemas"
};

string json = JsonConvert.SerializeObject(persona);
Console.WriteLine(json);
```

### Evidencia

Salida: `{"Nombre":"Cristell","Edad":20,"Carrera":"Ingenieria en Sistemas"}`

![Parte 2 – JSON](capturas/02_json.png)

---

## 6. Parte 3 – Login con BCrypt y SQL Server (20 pts)

**Enunciado:** crear un login que use el algoritmo **BCrypt** (biblioteca `BCrypt.Net-Next`), con base de datos de usuarios y una tabla de intentos exitosos y fallidos.

### 6.1 Base de datos (`BaseDatos/`)

El script `CrearBaseDatos.sql` crea la base `LoginBCryptDB` con dos tablas:

| Tabla | Columnas | Propósito |
|---|---|---|
| `Usuarios` | `Id`, `Usuario` (único), `NombreCompleto`, `PasswordHash`, `FechaCreacion` | Guarda el usuario y **solo el hash** de la contraseña |
| `IntentosLogin` | `Id`, `Usuario`, `Exitoso` (bit), `Fecha`, `Detalle` | Registra cada intento (exitoso o fallido) para análisis posterior |

También se incluye el backup `LoginBCryptDB.bak` (requisito: subir el backup de la BD al repositorio).

### 6.2 Funcionamiento

1. **Registrar:** `BCrypt.HashPassword(password, workFactor: 11)` genera el hash con sal aleatoria y se guarda en `Usuarios.PasswordHash`. La contraseña nunca se almacena en texto plano y se escribe oculta con `*`.
2. **Iniciar sesión:** se busca el hash del usuario y se compara con `BCrypt.Verify(password, hash)`.
3. **Registro de intentos:** cada intento se guarda en `IntentosLogin` con el usuario, la fecha, el resultado y un detalle (`Acceso concedido`, `Contraseña incorrecta` o `Usuario no existe`).
4. **Acceso exitoso:** el programa muestra el nombre del usuario.

```csharp
// Generación del hash
string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

// Verificación
bool exitoso = hashGuardado != null && BCrypt.Net.BCrypt.Verify(password, hashGuardado);

if (exitoso)
    Console.WriteLine($"¡Acceso exitoso! Bienvenido(a), {nombreCompleto} (usuario: {usuario}).");
else
    Console.WriteLine("Usuario o contraseña incorrectos.");   // mensaje genérico
```

**Medidas de seguridad aplicadas:** consultas parametrizadas (evitan inyección SQL), mensaje de error genérico (no revela si el usuario existe) y contraseña oculta en consola.

### 6.3 Evidencias

**Menú principal y registro de usuario (hash generado – 5 pts).** Se genera el hash BCrypt (`$2a$11$...`) y se guarda en la base de datos.

![Registro de usuario](capturas/03_registro.png)

**Intento fallido.** `BCrypt.Verify()` rechaza la contraseña incorrecta y el intento queda registrado.

![Login fallido](capturas/04_login_fallido.png)

**Acceso exitoso (verificación con `BCrypt.Verify()` y nombre del usuario – 10 pts).** La contraseña original es aceptada y el programa muestra el nombre: *Cristell Gonzalez*.

![Login exitoso](capturas/05_login_exitoso.png)

**Historial de intentos.** La opción 3 lista los intentos con fecha, usuario, resultado y detalle.

![Historial de intentos](capturas/06_historial.png)

**Contenido de la base de datos.** `Usuarios` guarda únicamente el hash; `IntentosLogin` guarda los intentos exitosos y fallidos.

![Base de datos](capturas/07_base_datos.png)

### 6.4 Consulta de análisis

```sql
SELECT Usuario,
       SUM(CASE WHEN Exitoso = 1 THEN 1 ELSE 0 END) AS Exitosos,
       SUM(CASE WHEN Exitoso = 0 THEN 1 ELSE 0 END) AS Fallidos
FROM dbo.IntentosLogin
GROUP BY Usuario;
```

---

## 7. Cómo ejecutar el proyecto

### Partes 1 y 2

```bash
dotnet run --project Parte1_Figuras
dotnet run --project Parte2_Json
```

### Parte 3

**a) Crear la base de datos** (elegir una opción):

```bash
# Opción 1: desde el script
sqlcmd -S "(localdb)\MSSQLLocalDB" -i BaseDatos/CrearBaseDatos.sql
```

```sql
-- Opción 2: restaurar el backup
RESTORE DATABASE LoginBCryptDB
FROM DISK = N'<ruta>\BaseDatos\LoginBCryptDB.bak' WITH REPLACE;
```

(En SSMS: *Restore Database → Device*.)

**b) Ejecutar el programa:**

```bash
dotnet run --project Parte3_LoginBCrypt
```

**c) Ver el contenido de las tablas:**

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LoginBCryptDB -i BaseDatos/ConsultarDatos.sql
```

> La cadena de conexión está en `Parte3_LoginBCrypt/Program.cs` (por defecto `(localdb)\MSSQLLocalDB`). Cámbiala si usas otra instancia de SQL Server.

**Usuario de prueba incluido en el backup:** `cristell` / `Clave123!`

---

## 8. Resumen de cumplimiento

| Parte | Requisito | Evidencia |
|---|---|---|
| 1 | Clase abstracta `Figura` | Sección 4 · `Parte1_Figuras/Program.cs` |
| 1 | Métodos abstractos desarrollados en los hijos | Sección 4 · `Rectangulo`, `Cuadrado`, `Triangulo` |
| 1 | Arreglo `Figura[]` | Sección 4 · `capturas/01_figuras.png` |
| 1 | Recorrido polimórfico | Sección 4 · `capturas/01_figuras.png` |
| 2 | Script JSON ejecutado | Sección 5 · `capturas/02_json.png` |
| 3 | Login con BCrypt y repositorio con backup de la BD | `Parte3_LoginBCrypt/` + `BaseDatos/LoginBCryptDB.bak` |
| 3 | Tablas de usuarios e intentos (exitosos/fallidos, usuario y fecha) | `CrearBaseDatos.sql` · `capturas/07_base_datos.png` |
| 3 | Hash de una palabra clave en la BD | `capturas/03_registro.png` |
| 3 | Verificación con `BCrypt.Verify()` | `capturas/04_login_fallido.png` · `capturas/05_login_exitoso.png` |
| 3 | Indicar el nombre del usuario al entrar | `capturas/05_login_exitoso.png` |
