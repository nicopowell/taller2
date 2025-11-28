using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;  
using MVC.Models; // Clases Modelo
using MVC.Interfaces;

namespace MVC.Repositorios;

    // Se mantiene la conexión en una constante para simplificar
    public class PresupuestoRepository: IPresupuestoRepository
    {
        // ❗ Cadena de conexión a la base de datos Sqlite (asumiendo tienda.db)
        private readonly string CadenaConexion = "Data Source=./DB/tienda.db";

        // Mantenemos Acoplamiento Fuerte: El Repositorio de Presupuestos
        // instancia al Repositorio de Productos para obtener los detalles.
        private readonly ProductoRepository _productoRepo = new ProductoRepository();

        // ------------------------------------------------------------------
        // OPERACIONES CRUD BÁSICAS
        // ------------------------------------------------------------------

        // 1. OBTENER TODOS
        public List<Presupuesto> GetAll()
        {
            var lista = new List<Presupuesto>();
            const string sql = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var presupuesto = new Presupuesto
                {
                    IdPresupuesto = lector.GetInt32(0),
                    NombreDestinatario = lector.GetString(1),
                    FechaCreacion = lector.GetDateTime(2),
                    // Nota: Los detalles (Detalle) NO se cargan en GetAll() por eficiencia.
                    // Se cargarán solo en GetById() para mantener la lógica síncrona simple.
                };
                lista.Add(presupuesto);
            }

            return lista;
        }

        // 2. OBTENER POR ID
        public Presupuesto GetById(int id)
        {
            Presupuesto presupuesto = null;
            const string sql = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE IdPresupuesto = @Id";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            using var lector = comando.ExecuteReader();

            if (lector.Read())
            {
                presupuesto = new Presupuesto
                {
                    IdPresupuesto = lector.GetInt32(0),
                    NombreDestinatario = lector.GetString(1),
                    FechaCreacion = lector.GetDateTime(2)
                };
                
                // ❗ Lógica Crítica: Cargar los detalles del presupuesto
                presupuesto.Detalle = GetDetallesByPresupuestoId(id);
            }

            return presupuesto;
        }

        // 3. AGREGAR
        public void Add(Presupuesto presupuesto)
        {
            const string sql = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
            comando.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);

            comando.ExecuteNonQuery();
        }

        // 4. ACTUALIZAR
        public void Update(Presupuesto presupuesto)
        {
            const string sql = "UPDATE Presupuestos SET NombreDestinatario = @NombreDestinatario, FechaCreacion = @FechaCreacion WHERE IdPresupuesto = @Id";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
            comando.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);
            comando.Parameters.AddWithValue("@Id", presupuesto.IdPresupuesto);

            comando.ExecuteNonQuery();
        }

        // 5. ELIMINAR
        public void Delete(int id)
        {
            // Nota: En una aplicación real, se debería usar una transacción y primero
            // eliminar los detalles (PresupuestoDetalle) para evitar errores de FK.
            
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            // 1. Eliminar los detalles asociados
            const string sqlDetalle = "DELETE FROM PresupuestoDetalle WHERE IdPresupuesto = @Id";
            using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion);
            comandoDetalle.Parameters.AddWithValue("@Id", id);
            comandoDetalle.ExecuteNonQuery();
            
            // 2. Eliminar el presupuesto
            const string sqlPresupuesto = "DELETE FROM Presupuestos WHERE IdPresupuesto = @Id";
            using var comandoPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);
            comandoPresupuesto.Parameters.AddWithValue("@Id", id);
            comandoPresupuesto.ExecuteNonQuery();
        }

        // ------------------------------------------------------------------
        // LÓGICA RELACIONAL (Detalle y N:M)
        // ------------------------------------------------------------------

        // Método auxiliar para cargar los detalles de un presupuesto (usado en GetById)
        private List<PresupuestoDetalle> GetDetallesByPresupuestoId(int idPresupuesto)
        {
            var detalles = new List<PresupuestoDetalle>();
            const string sql = "SELECT IdPresupuesto, IdProducto, Cantidad FROM PresupuestoDetalle WHERE IdPresupuesto = @IdPresupuesto";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

            using var lector = comando.ExecuteReader();

            // Cargamos todos los productos de la BD de una vez para optimizar
            var todosLosProductos = _productoRepo.GetAll().ToDictionary(p => p.IdProducto);

            while (lector.Read())
            {
                int idProducto = lector.GetInt32(1);
                
                // Mapeo
                var detalle = new PresupuestoDetalle
                {
                    // Buscamos el objeto Producto en el diccionario cargado previamente
                    Producto = todosLosProductos.ContainsKey(idProducto) ? todosLosProductos[idProducto] : null,
                    Cantidad = lector.GetInt32(2)
                };
                
                if (detalle.Producto != null)
                {
                    detalles.Add(detalle);
                }
            }

            return detalles;
        }

        // ❗ MÉTODO CLAVE DEL TP: Agrega un registro a la tabla de relación N:M
        public void AddDetalle(int idPresupuesto, int idProducto, int cantidad)
        {
            const string sql = "INSERT INTO PresupuestoDetalle (IdPresupuesto, IdProducto, Cantidad) VALUES (@IdPresupuesto, @IdProducto, @Cantidad)";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
            comando.Parameters.AddWithValue("@IdProducto", idProducto);
            comando.Parameters.AddWithValue("@Cantidad", cantidad);

            comando.ExecuteNonQuery();
        }
    }
