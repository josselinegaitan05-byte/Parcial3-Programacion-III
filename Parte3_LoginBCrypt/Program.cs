// Parcial #3 - Parte 3: Login con hash BCrypt (BCrypt.Net-Next) y SQL Server
using Microsoft.Data.SqlClient;

namespace Parte3_LoginBCrypt
{
    internal class Program
    {
        // Cadena de conexión a SQL Server LocalDB (cambiar si se usa otra instancia)
        private const string Conexion =
            @"Server=(localdb)\MSSQLLocalDB;Database=LoginBCryptDB;Trusted_Connection=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n===== SISTEMA DE LOGIN (BCrypt) =====");
                Console.WriteLine("1. Registrar usuario");
                Console.WriteLine("2. Iniciar sesión");
                Console.WriteLine("3. Ver historial de intentos");
                Console.WriteLine("4. Salir");
                Console.Write("Opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": Registrar(); break;
                    case "2": IniciarSesion(); break;
                    case "3": VerIntentos(); break;
                    case "4":
                    case null: return;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        // ---------- REGISTRO: genera el hash BCrypt y lo guarda en la BD ----------
        static void Registrar()
        {
            Console.Write("Usuario: ");
            string usuario = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Nombre completo: ");
            string nombre = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Contraseña: ");
            string password = LeerPassword();

            if (usuario == "" || nombre == "" || password == "")
            {
                Console.WriteLine("Todos los campos son obligatorios.");
                return;
            }

            // Generación del hash BCrypt (incluye una sal aleatoria; workFactor 11 = 2^11 rondas)
            string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
            Console.WriteLine($"Hash generado: {hash}");

            try
            {
                using var cn = new SqlConnection(Conexion);
                cn.Open();
                using var cmd = new SqlCommand(
                    "INSERT INTO dbo.Usuarios (Usuario, NombreCompleto, PasswordHash) VALUES (@u, @n, @h)", cn);
                cmd.Parameters.AddWithValue("@u", usuario);
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Usuario registrado correctamente.");
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                Console.WriteLine("Ese nombre de usuario ya existe.");
            }
        }

        // ---------- LOGIN: verifica con BCrypt.Verify() y registra el intento ----------
        static void IniciarSesion()
        {
            Console.Write("Usuario: ");
            string usuario = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Contraseña: ");
            string password = LeerPassword();

            using var cn = new SqlConnection(Conexion);
            cn.Open();

            string? hashGuardado = null;
            string? nombreCompleto = null;

            using (var cmd = new SqlCommand(
                "SELECT PasswordHash, NombreCompleto FROM dbo.Usuarios WHERE Usuario = @u", cn))
            {
                cmd.Parameters.AddWithValue("@u", usuario);
                using var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    hashGuardado = dr.GetString(0);
                    nombreCompleto = dr.GetString(1);
                }
            }

            bool exitoso = hashGuardado != null && BCrypt.Net.BCrypt.Verify(password, hashGuardado);
            string detalle = hashGuardado == null ? "Usuario no existe"
                           : exitoso ? "Acceso concedido" : "Contraseña incorrecta";

            RegistrarIntento(cn, usuario, exitoso, detalle);

            if (exitoso)
            {
                Console.WriteLine($"\n¡Acceso exitoso! Bienvenido(a), {nombreCompleto} (usuario: {usuario}).");
            }
            else
            {
                // Mensaje genérico para no revelar si el usuario existe
                Console.WriteLine("\nUsuario o contraseña incorrectos.");
            }
        }

        static void RegistrarIntento(SqlConnection cn, string usuario, bool exitoso, string detalle)
        {
            using var cmd = new SqlCommand(
                "INSERT INTO dbo.IntentosLogin (Usuario, Exitoso, Detalle) VALUES (@u, @e, @d)", cn);
            cmd.Parameters.AddWithValue("@u", usuario);
            cmd.Parameters.AddWithValue("@e", exitoso);
            cmd.Parameters.AddWithValue("@d", detalle);
            cmd.ExecuteNonQuery();
        }

        static void VerIntentos()
        {
            using var cn = new SqlConnection(Conexion);
            cn.Open();
            using var cmd = new SqlCommand(
                "SELECT TOP 20 Fecha, Usuario, Exitoso, Detalle FROM dbo.IntentosLogin ORDER BY Fecha DESC", cn);
            using var dr = cmd.ExecuteReader();

            Console.WriteLine($"\n{"Fecha",-21}{"Usuario",-15}{"Resultado",-11}Detalle");
            Console.WriteLine(new string('-', 70));
            while (dr.Read())
            {
                Console.WriteLine($"{dr.GetDateTime(0):yyyy-MM-dd HH:mm:ss}  {dr.GetString(1),-15}" +
                                  $"{(dr.GetBoolean(2) ? "EXITOSO" : "FALLIDO"),-11}{dr.GetString(3)}");
            }
        }

        // Lee la contraseña mostrando '*' (si la entrada está redirigida, lee la línea normal)
        static string LeerPassword()
        {
            if (Console.IsInputRedirected)
                return Console.ReadLine() ?? "";

            var sb = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0) { sb.Length--; Console.Write("\b \b"); }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            return sb.ToString();
        }
    }
}
