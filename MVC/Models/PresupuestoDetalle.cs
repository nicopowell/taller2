using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    // ====================================================================================
    // MODELO DE DOMINIO: DETALLE DE PRESUPUESTO (Item)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Relación Muchos a Muchos (N:M) & Composición
    //
    // 1. ¿Qué representa?
    //    Es un renglón individual dentro de un presupuesto.
    //    Ejemplo: "3 unidades de Coca Cola".
    //
    // 2. Función en la Base de Datos:
    //    Representa la tabla intermedia 'PresupuestoDetalle' que rompe la relación N:M
    //    entre 'Presupuestos' y 'Productos'.
    // ====================================================================================

    public class PresupuestoDetalle
    {
        // Identificador único del renglón (PK de la tabla detalle).
        public int IdPresupuestoDetalle { get; set; } 
        
        // --------------------------------------------------------------------------------
        // RELACIÓN CON PRODUCTO
        // --------------------------------------------------------------------------------
        // En lugar de guardar solo el 'IdProducto' (FK), guardamos el objeto completo 'Producto'.
        // Esto nos permite acceder a sus propiedades (Nombre, Precio) fácilmente desde la vista
        // sin tener que hacer consultas adicionales manuales.
        // El repositorio se encargará de "hidratar" (llenar) esta propiedad al leer de la BD.
        public Producto Producto { get; set; }
        
        // --------------------------------------------------------------------------------
        // DATOS PROPIOS DEL DETALLE
        // --------------------------------------------------------------------------------
        // La cantidad es un atributo que no pertenece ni al Producto ni al Presupuesto por separado,
        // sino a la relación entre ambos.
        public int Cantidad { get; set; } 
    }
}