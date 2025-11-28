using Microsoft.Data.Sqlite;
using MVC.Interfaces;
using MVC.Models; // Asume que la clase Usuario está en la carpeta Models

namespace MVC.Repositorios;

    // ❗ Implementa la interfaz para permitir la Inyección de Dependencias
    public class UsuarioRepository : IUserRepository
    {
        // Cadena de conexión SQLite
        // Asegúrate de que esta ruta sea correcta para tu proyecto
        private readonly string CadenaConexion = "Data Source=./DB/tienda.db";

        // Constructor vacío (la Inyección de Dependencias se aplicará en el Controller)
        public Usuario GetUser(string usuario, string contrasena)
        {
            Usuario user = null;

            // ❗ Consulta SQL que busca por Usuario Y Contrasena
            const string sql = @"
                SELECT Id, Nombre, User, Pass, Rol 
                FROM Usuarios 
                WHERE User = @Usuario AND Pass = @Contrasena";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            
            // ❗ Se usan parámetros para prevenir inyección SQL
            comando.Parameters.AddWithValue("@Usuario", usuario);
            comando.Parameters.AddWithValue("@Contrasena", contrasena);

            using var reader = comando.ExecuteReader();

            if (reader.Read())
            {
                // Si el lector encuentra una fila, el usuario existe y las credenciales son correctas
                user = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    User = reader.GetString(2),
                    Pass = reader.GetString(3),
                    Rol = reader.GetString(4) // ❗ El Rol es crucial para la Autorización (Punto 3.e)
                };
            }
            
            return user;
        }
    }
