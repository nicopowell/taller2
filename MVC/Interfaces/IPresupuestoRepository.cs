using System.Collections.Generic;
using MVC.Models; // Asegúrate de tener los using correctos

namespace MVC.Interfaces
{
    // CUMPLE DI: Abstracción para el Repositorio de Presupuestos
    public interface IPresupuestoRepository
    {
        List<Presupuesto> GetAll();
        
        Presupuesto GetById(int id);
        
        void Add(Presupuesto presupuesto);
        
        void Update(Presupuesto presupuesto);
        
        void Delete(int id);
        
        // Método clave del TP para la relación N:M
        void AddDetalle(int idPresupuesto, int idProducto, int cantidad);
    }
}