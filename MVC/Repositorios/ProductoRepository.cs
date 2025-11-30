using Microsoft.Data.Sqlite;
using MVC.Models;
using MVC.Interfaces;

namespace MVC.Repositorios
{
    // ====================================================================================
    // REPOSITORIO DE PRODUCTOS (ADO.NET + SQLITE)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Data Access Layer (DAL) & Implementación de Interfaces (TP 10)
    //
    // 1. ¿Qué hace esta clase?
    //    Es la implementación CONCRETA del acceso a datos. Aquí escribimos SQL.
    //    Implementa la interfaz 'IProductoRepository', obligándose a cumplir su contrato.
    //
    // 2. ¿Por qué ADO.NET?
    //    Usamos las clases nativas (SqliteConnection, SqliteCommand) para tener control total
    //    y eficiencia. Es la forma más "cruda" y rápida de acceder a una base de datos en .NET.
    // ====================================================================================

    public class ProductoRepository : IProductoRepository
    {
        // Cadena de conexión. En un proyecto real (Prod), esto iría en appsettings.json.
        // Aquí la hardcodeamos por simplicidad del TP.
        // "Data Source" indica la ruta del archivo de base de datos SQLite.
        private readonly string CadenaConexion = "Data Source=./DB/tienda.db";

        // Constructor vacío.
        // Al registrarse como Scoped en Program.cs, el contenedor de DI crea una instancia nueva
        // de esta clase cada vez que llega una petición HTTP que la necesite.
        public ProductoRepository()
        {
        }

        // --------------------------------------------------------------------------------
        // LECTURA: OBTENER TODOS LOS PRODUCTOS
        // --------------------------------------------------------------------------------
        public List<Producto> GetAll()
        {
            var listaProductos = new List<Producto>();

            // 1. CREAR CONEXIÓN
            // El bloque 'using' asegura que la conexión se cierre y se libere la memoria (Dispose)
            // automáticamente al terminar el bloque, incluso si hay errores.
            using var conexion = new SqliteConnection(CadenaConexion);
            
            // 2. ABRIR CONEXIÓN
            // Es necesario abrirla explícitamente antes de ejecutar comandos.
            conexion.Open();

            // 3. DEFINIR COMANDO SQL
            string sql = "SELECT IdProducto, Descripcion, Precio FROM Productos";
            using var comando = new SqliteCommand(sql, conexion);
            
            // 4. EJECUTAR LECTURA (ExecuteReader)
            // 'ExecuteReader' devuelve un flujo de datos (stream) de solo avance.
            // Es muy eficiente porque no carga todo en memoria de golpe, sino fila por fila.
            using var reader = comando.ExecuteReader();

            // 5. LEER RESULTADOS
            // 'reader.Read()' avanza a la siguiente fila. Devuelve false cuando no hay más.
            while (reader.Read())
            {
                listaProductos.Add(new Producto
                {
                    // Mapeo manual: Columna de BD -> Propiedad del Objeto
                    // GetInt32(0) lee la primera columna del SELECT (IdProducto)
                    IdProducto = reader.GetInt32(0),
                    Descripcion = reader.GetString(1), // Segunda columna
                    Precio = reader.GetDecimal(2)      // Tercera columna
                });
            }
            // Retornamos la lista llena (o vacía si no había datos).
            return listaProductos;
        }

        // --------------------------------------------------------------------------------
        // LECTURA: OBTENER UN PRODUCTO POR ID
        // --------------------------------------------------------------------------------
        public Producto GetById(int id)
        {
            Producto producto = null;

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            // Consulta parametrizada con @Id
            string sql = "SELECT IdProducto, Descripcion, Precio FROM Productos WHERE IdProducto = @Id";
            using var comando = new SqliteCommand(sql, conexion);
            
            // IMPORTANTE: Uso de Parámetros (@Id)
            // Nunca concatenar strings (ej: "... WHERE Id = " + id).
            // Los parámetros previenen ataques de Inyección SQL y manejan los tipos de datos correctamente.
            comando.Parameters.AddWithValue("@Id", id);

            using var reader = comando.ExecuteReader();

            // Usamos 'if' en lugar de 'while' porque esperamos un solo resultado (o ninguno).
            if (reader.Read())
            {
                producto = new Producto
                {
                    IdProducto = reader.GetInt32(0),
                    Descripcion = reader.GetString(1),
                    Precio = reader.GetDecimal(2)
                };
            }
            return producto; // Puede devolver null si no se encontró el ID.
        }

        // --------------------------------------------------------------------------------
        // ESCRITURA: CREAR NUEVO (ALTA)
        // --------------------------------------------------------------------------------
        public void Add(Producto producto)
        {
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            // INSERT no incluye el IdProducto porque es AUTOINCREMENT en SQLite.
            string sql = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
            using var comando = new SqliteCommand(sql, conexion);

            // Mapeamos las propiedades del objeto a los parámetros SQL.
            comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            comando.Parameters.AddWithValue("@Precio", producto.Precio);

            // 'ExecuteNonQuery': Se usa para INSERT, UPDATE, DELETE.
            // No devuelve filas, solo el número de filas afectadas (que aquí ignoramos).
            comando.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------------------
        // ESCRITURA: MODIFICAR EXISTENTE (UPDATE)
        // --------------------------------------------------------------------------------
        public void Update(Producto producto)
        {
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            string sql = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE IdProducto = @Id";
            using var comando = new SqliteCommand(sql, conexion);

            comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            comando.Parameters.AddWithValue("@Precio", producto.Precio);
            comando.Parameters.AddWithValue("@Id", producto.IdProducto); // Necesario para el WHERE

            comando.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------------------
        // ESCRITURA: ELIMINAR (DELETE)
        // --------------------------------------------------------------------------------
        public void Delete(int id)
        {
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            string sql = "DELETE FROM Productos WHERE IdProducto = @Id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
        }
    }
}