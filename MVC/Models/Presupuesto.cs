using System;
using System.Collections.Generic;
using System.Linq; // Necesario para usar .Sum()

namespace MVC.Models
{
    // ====================================================================================
    // MODELO DE DOMINIO: PRESUPUESTO (Entidad Principal)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Entidades (Tema 12) & Composición (POO - Tema 02)
    //
    // 1. ¿Qué representa?
    //    Es la cabecera de un presupuesto. Contiene los datos generales (quién y cuándo).
    //
    // 2. Relación (Composición/Agregación):
    //    Un Presupuesto "TIENE MUCHOS" Detalles (Items). 
    //    Esta relación se modela con una lista: List<PresupuestoDetalle>.
    // ====================================================================================

    public class Presupuesto
    {
        private const decimal IVA = 0.21m; // Constante de negocio (21% IVA).

        // --------------------------------------------------------------------------------
        // PROPIEDADES (Datos)
        // --------------------------------------------------------------------------------
        
        // Clave Primaria (PK) en la base de datos.
        public int IdPresupuesto { get; set; }
        
        // Nombre del cliente o destinatario.
        public string NombreDestinatario { get; set; }
        
        // Fecha de emisión.
        public DateTime FechaCreacion { get; set; }

        // ❗ PROPIEDAD DE NAVEGACIÓN (Relación 1:N) ❗
        // Representa los renglones del presupuesto.
        // Se inicializa con 'new List<...>()' para evitar NullReferenceException al agregar ítems.
        public List<PresupuestoDetalle> Detalle { get; set; } = new List<PresupuestoDetalle>();

        // --------------------------------------------------------------------------------
        // MÉTODOS DE LÓGICA DE NEGOCIO (POO - Encapsulamiento)
        // --------------------------------------------------------------------------------
        // En lugar de calcular totales en el Controlador, la Entidad sabe calcularse a sí misma.
        // Esto cumple con el principio de que los objetos deben ser responsables de su propio estado.

        /// <summary>
        /// Calcula el monto total del presupuesto sumando los subtotales de sus detalles.
        /// (Precio * Cantidad) de cada ítem.
        /// </summary>
        public decimal MontoPresupuesto()
        {
            // Usamos LINQ 'Sum' para iterar sobre la lista 'Detalle'.
            // d representa un objeto PresupuestoDetalle.
            // Se asume que 'd.Producto' está cargado (no es null).
            return Detalle.Sum(d => d.Producto.Precio * d.Cantidad);
        }

        /// <summary>
        /// Calcula el monto total aplicando el IVA.
        /// </summary>
        public decimal MontoPresupuestoConIva()
        {
            decimal montoBase = MontoPresupuesto();
            return montoBase * (1 + IVA);
        }

        /// <summary>
        /// Cuenta la cantidad total de unidades de productos (bultos).
        /// </summary>
        public int CantidadProductos()
        {
            return Detalle.Sum(d => d.Cantidad);
        }
    }
}