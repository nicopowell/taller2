using Microsoft.Data.Sqlite;
using MVC.Interfaces;
using MVC.Models; 

namespace MVC.Repositorios
{
    // ====================================================================================
    // REPOSITORIO DE USUARIOS (Acceso a Datos de Autenticación)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Patrón Repositorio (TP 10) & Seguridad
    //
    // 1. Objetivo:
    //    Implementar la lógica concreta para buscar usuarios en la base de datos SQLite.
    //    Cumple con el contrato definido en 'IUserRepository'.
    //
    // 2. Inyección de Dependencias:
    //    Esta clase se registra en Program.cs como la implementación concreta de IUserRepository.
    // ====================================================================================

    public class UsuarioRepository : IUserRepository
    {
        // Cadena de conexión a la base de datos (Hardcoded para el TP).
        private readonly string CadenaConexion = "Data Source=./DB/tienda.db";

        // Constructor vacío (la Inyección de Dependencias se aplica al INYECTAR esta clase, no aquí).
        public UsuarioRepository()
        {
        }

        // --------------------------------------------------------------------------------
        // MÉTODO: OBTENER USUARIO (LOGIN)
        // --------------------------------------------------------------------------------
        // Busca un usuario que coincida EXACTAMENTE con el nombre de usuario y la contraseña.
        public Usuario GetUser(string usuario, string contrasena)
        {
            Usuario user = null;

            // CONSULTA SQL:
            // Seleccionamos todos los datos necesarios para construir el objeto Usuario.
            // Filtramos por User y Pass.
            // NOTA: En un sistema real, la contraseña debería estar hasheada y se compararía el hash.
            const string sql = @"
                SELECT Id, Nombre, User, Pass, Rol 
                FROM Usuarios 
                WHERE User = @Usuario AND Pass = @Contrasena";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            
            // ❗ SEGURIDAD CRÍTICA: USO DE PARÁMETROS ❗
            // NUNCA concatenar strings en una consulta de Login (ej: "... WHERE User = '" + usuario + "'").
            // Eso permitiría ataques de Inyección SQL (ej: entrar con "' OR '1'='1").
            // Los parámetros (@Usuario, @Contrasena) aseguran que los valores se traten como datos, no código.
            comando.Parameters.AddWithValue("@Usuario", usuario);
            comando.Parameters.AddWithValue("@Contrasena", contrasena);

            using var reader = comando.ExecuteReader();

            // Si reader.Read() devuelve true, significa que encontró UN registro (login exitoso).
            if (reader.Read())
            {
                user = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    User = reader.GetString(2),
                    Pass = reader.GetString(3),
                    
                    // ❗ ROL: Dato crucial que se guardará en la sesión para autorizar acciones.
                    Rol = reader.GetString(4) 
                };
            }
            
            // Si no se encontró nada, 'user' sigue siendo null, lo que indicará al servicio que el login falló.
            return user;
        }
    }
}