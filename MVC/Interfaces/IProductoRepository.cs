using System.Collections.Generic;
using MVC.Models; // Asume que Producto está en la carpeta Models

namespace MVC.Interfaces
{
    // CUMPLE DI: Abstracción del Repositorio de Productos
    public interface IProductoRepository
    {
        // El método GetAll devuelve una lista de Producto
        List<Producto> GetAll();
        
        // El método GetById devuelve un único Producto o null
        Producto GetById(int id);
        
        // El método Add recibe un Producto para dar de alta
        void Add(Producto producto);
        
        // El método Update recibe un Producto para modificar
        void Update(Producto producto);
        
        // El método Delete recibe el ID del producto a eliminar
        void Delete(int id);
    }
}