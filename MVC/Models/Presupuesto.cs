using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC.Models;

    public class Presupuesto
    {
        private const decimal IVA = 0.21m; // 21% de IVA

        // Propiedades de la Entidad
        public int IdPresupuesto { get; set; }
        public string NombreDestinatario { get; set; }
        public DateTime FechaCreacion { get; set; }

        // ❗ Propiedad Relacional (Colección de ítems)
        public List<PresupuestoDetalle> Detalle { get; set; } = new List<PresupuestoDetalle>();

        // ------------------------------------------------------------------
        // MÉTODOS DE LÓGICA DE NEGOCIO REQUERIDOS
        // ------------------------------------------------------------------
        
        /// <summary>
        /// Calcula el monto total del presupuesto SIN IVA.
        /// </summary>
        /// <returns>Monto total base.</returns>
        public decimal MontoPresupuesto()
        {
            // Se calcula sumando el subtotal de cada detalle (Precio * Cantidad)
            return Detalle.Sum(d => d.Producto.Precio * d.Cantidad);
        }

        /// <summary>
        /// Calcula el monto total del presupuesto CON IVA (21%).
        /// </summary>
        /// <returns>Monto total con IVA incluido.</returns>
        public decimal MontoPresupuestoConIva()
        {
            decimal montoBase = MontoPresupuesto();
            // Retorna el monto base más el 21% del IVA
            return montoBase * (1 + IVA);
        }

        /// <summary>
        /// Cuenta el total de productos sumando las cantidades de todos los ítems.
        /// </summary>
        /// <returns>Cantidad total de unidades de productos.</returns>
        public int CantidadProductos()
        {
            // Suma la propiedad Cantidad de cada elemento en la lista Detalle
            return Detalle.Sum(d => d.Cantidad);
        }
    }
