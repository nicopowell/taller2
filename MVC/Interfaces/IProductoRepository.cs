using System.Collections.Generic;
using MVC.Models; 

namespace MVC.Interfaces
{
    // ====================================================================================
    // INTERFAZ DEL REPOSITORIO DE PRODUCTOS
    // ====================================================================================
    // CONCEPTO TEÓRICO: Patrón Repositorio (Tema 09) & Inyección de Dependencias (TP 10)
    // 
    // 1. ¿Qué es esto? 
    //    Es un "Contrato". Define QUÉ puede hacer el repositorio de productos, 
    //    pero no CÓMO lo hace. No hay código SQL ni lógica aquí, solo firmas de métodos.
    //
    // 2. ¿Para qué sirve en el Examen?
    //    - Desacoplamiento: El controlador (ProductosController) hablará con esta interfaz,
    //      no con la clase concreta (ProductoRepository).
    //    - Inyección de Dependencias (DI): En Program.cs registramos:
    //      "builder.Services.AddScoped<IProductoRepository, ProductoRepository>();"
    //      Esto permite que .NET inyecte la implementación real cuando se pida la interfaz.
    //    - Testabilidad: Permite crear "MockRepositories" falsos para pruebas unitarias.
    // ====================================================================================

    public interface IProductoRepository
    {
        // --- LECTURA (READ) ---
        
        // Devuelve todos los productos. 
        // Usado en la acción Index del controlador.
        List<Producto> GetAll();
        
        // Busca un producto específico por su ID único.
        // Usado en las acciones Details, Edit (Get) y Delete (Get) para buscar antes de mostrar.
        Producto GetById(int id);
        
        // --- ESCRITURA (CREATE, UPDATE, DELETE) ---
        
        // Recibe un objeto Producto (normalmente mapeado desde un ViewModel) y lo guarda en la BD.
        // Usado en la acción Create (Post).
        void Add(Producto producto);
        
        // Recibe un objeto Producto con los datos modificados y actualiza el registro existente.
        // Usado en la acción Edit (Post).
        void Update(Producto producto);
        
        // Elimina el registro correspondiente al ID proporcionado.
        // Usado en la acción Delete (Post).
        void Delete(int id);
    }
}