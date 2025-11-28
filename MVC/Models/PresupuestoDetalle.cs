namespace MVC.Models;

    // Esta clase representa un ítem dentro de un presupuesto (la relación N:M)
    public class PresupuestoDetalle
    {
        // No siempre es necesario, pero es bueno si la tabla lo tiene
        public int IdPresupuestoDetalle { get; set; } 
        
        // Relación con Producto
        public Producto Producto { get; set; }
        
        // Cantidad de ese producto en este presupuesto
        public int Cantidad { get; set; } 
    }
