using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;  
using MVC.Models; 
using MVC.Interfaces;

namespace MVC.Repositorios
{
    // ====================================================================================
    // REPOSITORIO DE PRESUPUESTOS (Lógica de Negocio y Datos)
    // ====================================================================================
    // CONCEPTO TEÓRICO: ADO.NET Avanzado & Manejo de Relaciones
    //
    // 1. Complejidad: A diferencia de ProductoRepository, este repositorio maneja
    //    una entidad COMPUESTA (Presupuesto + Lista de Detalles).
    //
    // 2. Estrategia de Carga:
    //    - GetAll(): Carga "Lazy" (Perezosa) o ligera. Solo trae cabeceras para el listado.
    //    - GetById(): Carga "Eager" (Ansiosa). Trae la cabecera Y todos sus detalles.
    // ====================================================================================

    public class PresupuestoRepository : IPresupuestoRepository
    {
        // Cadena de conexión (Hardcoded para el TP).
        private readonly string CadenaConexion = "Data Source=./DB/tienda.db";

        // --------------------------------------------------------------------------------
        // DEPENDENCIA DE PRODUCTOS
        // --------------------------------------------------------------------------------
        // NOTA DE ARQUITECTURA: 
        // Aquí estamos instanciando directamente 'new ProductoRepository()'.
        // En una arquitectura pura con Inyección de Dependencias (TP 10), esto debería 
        // inyectarse en el constructor (IProductoRepository).
        // Sin embargo, para simplificar la carga de detalles en este método, se usa así.
        private readonly ProductoRepository _productoRepo = new ProductoRepository();

        // --------------------------------------------------------------------------------
        // 1. OBTENER TODOS (LISTADO GENERAL)
        // --------------------------------------------------------------------------------
        public List<Presupuesto> GetAll()
        {
            var lista = new List<Presupuesto>();
            
            // Consulta SQL simple: Solo datos de la tabla principal.
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
                    // ❗ IMPORTANTE: No cargamos la lista 'Detalle' aquí.
                    // Esto mejora el rendimiento del listado principal (Index).
                    // Si el usuario quiere ver los productos, entrará a 'Details'.
                };
                lista.Add(presupuesto);
            }

            return lista;
        }

        // --------------------------------------------------------------------------------
        // 2. OBTENER POR ID (DETALLE COMPLETO)
        // --------------------------------------------------------------------------------
        public Presupuesto GetById(int id)
        {
            Presupuesto presupuesto = null;
            
            // Consulta para obtener la cabecera.
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
                
                // ❗ PUNTO CRÍTICO: HIDRATACIÓN DE RELACIONES ❗
                // Una vez que tenemos la cabecera, llamamos a un método auxiliar privado
                // para ir a buscar todos los renglones (items) que pertenecen a este presupuesto ID.
                presupuesto.Detalle = GetDetallesByPresupuestoId(id);
            }

            return presupuesto;
        }

        // --------------------------------------------------------------------------------
        // 3. AGREGAR (CREATE) - Solo Cabecera
        // --------------------------------------------------------------------------------
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

        // --------------------------------------------------------------------------------
        // 4. ACTUALIZAR (UPDATE) - Solo Cabecera
        // --------------------------------------------------------------------------------
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

        // --------------------------------------------------------------------------------
        // 5. ELIMINAR (DELETE) - Eliminación en Cascada Manual
        // --------------------------------------------------------------------------------
        public void Delete(int id)
        {
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            // INTEGRIDAD REFERENCIAL:
            // Antes de borrar el Presupuesto (Padre), debemos borrar sus Detalles (Hijos).
            // Si la BD no tiene configurado "ON DELETE CASCADE", hacerlo manual evita errores de FK.
            
            // Paso 1: Borrar hijos
            const string sqlDetalle = "DELETE FROM PresupuestoDetalle WHERE IdPresupuesto = @Id";
            using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion);
            comandoDetalle.Parameters.AddWithValue("@Id", id);
            comandoDetalle.ExecuteNonQuery();
            
            // Paso 2: Borrar padre
            const string sqlPresupuesto = "DELETE FROM Presupuestos WHERE IdPresupuesto = @Id";
            using var comandoPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);
            comandoPresupuesto.Parameters.AddWithValue("@Id", id);
            comandoPresupuesto.ExecuteNonQuery();
        }

        // ================================================================================
        // MÉTODOS AUXILIARES Y RELACIONES (LÓGICA COMPLEJA)
        // ================================================================================

        // Método privado para recuperar la lista de ítems de un presupuesto.
        private List<PresupuestoDetalle> GetDetallesByPresupuestoId(int idPresupuesto)
        {
            var detalles = new List<PresupuestoDetalle>();
            
            // Seleccionamos los datos de la tabla intermedia.
            const string sql = "SELECT IdPresupuesto, IdProducto, Cantidad FROM PresupuestoDetalle WHERE IdPresupuesto = @IdPresupuesto";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

            using var lector = comando.ExecuteReader();

            // OPTIMIZACIÓN DE RENDIMIENTO:
            // En lugar de hacer una consulta a la BD por CADA detalle para buscar el nombre del producto
            // (Problema N+1), traemos todos los productos a memoria UNA VEZ y usamos un Diccionario.
            // Dictionary<Key=Id, Value=Producto> permite búsquedas instantáneas.
            var todosLosProductos = _productoRepo.GetAll().ToDictionary(p => p.IdProducto);

            while (lector.Read())
            {
                int idProducto = lector.GetInt32(1); // Obtenemos el FK del producto
                
                // Construimos el objeto detalle
                var detalle = new PresupuestoDetalle
                {
                    // Buscamos el objeto Producto completo en nuestro diccionario en memoria.
                    // Si existe, lo asignamos; si no (ej: producto borrado), queda null.
                    Producto = todosLosProductos.ContainsKey(idProducto) ? todosLosProductos[idProducto] : null,
                    
                    Cantidad = lector.GetInt32(2)
                };
                
                // Solo agregamos si el producto todavía existe.
                if (detalle.Producto != null)
                {
                    detalles.Add(detalle);
                }
            }

            return detalles;
        }

        // --------------------------------------------------------------------------------
        // AGREGAR DETALLE (RELACIÓN N:M)
        // --------------------------------------------------------------------------------
        // Este método inserta en la tabla intermedia. Vincula un Presupuesto con un Producto.
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
}