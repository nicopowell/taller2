using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Necesario para [ValidateNever]

namespace MVC.ViewModels
{
    // ====================================================================================
    // VIEW MODEL: AGREGAR PRODUCTO (Relación N:M)
    // ====================================================================================
    // CONCEPTO TEÓRICO: ViewModels para Relaciones (TP 9)
    //
    // 1. Objetivo:
    //    Capturar los datos necesarios para vincular un Producto existente a un Presupuesto.
    //    No crea un producto nuevo, crea una relación (PresupuestoDetalle).
    //
    // 2. Desafío del SelectList:
    //    Este VM transporta la lista de opciones para el dropdown. Es un patrón común en MVC.
    // ====================================================================================

    public class AgregarProductoViewModel
    {
        // ID del Presupuesto al que vamos a agregar el ítem. 
        // Viaja en el formulario como <input type="hidden">.
        public int IdPresupuesto { get; set; }

        // --------------------------------------------------------------------------------
        // SELECCIÓN DEL PRODUCTO (Dropdown)
        // --------------------------------------------------------------------------------
        // Esta propiedad guardará el "value" seleccionado en el <select>.
        // [Required]: Obliga al usuario a elegir una opción válida.
        
        [Display(Name = "Producto a agregar")]
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int IdProducto { get; set; } 

        // --------------------------------------------------------------------------------
        // CANTIDAD (Reglas de Negocio)
        // --------------------------------------------------------------------------------
        // Validaciones típicas de negocio para evitar datos inconsistentes (ej: -5 productos).
        
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }
        
        // --------------------------------------------------------------------------------
        // LISTA DE OPCIONES PARA EL DROPDOWN
        // --------------------------------------------------------------------------------
        // CONCEPTOS CLAVE:
        // 1. SelectList: Objeto de ASP.NET Core que contiene los items <option> para el HTML.
        // 2. Ciclo de Vida (El problema del NULL):
        //    - GET: El controlador llena esta lista y manda el VM a la vista.
        //    - POST: El formulario SOLO envía 'IdProducto' seleccionado, NO envía la lista completa de vuelta.
        //    - RESULTADO: Cuando el modelo llega al controlador en el POST, 'ListaProductos' es NULL.
        //
        // 3. [ValidateNever]: Le dice al validador del servidor "No te preocupes si esto es null".
        //    Sin esto, ModelState.IsValid daría false porque la lista es null en el POST.
        
        [ValidateNever] 
        public SelectList? ListaProductos { get; set; } 
    }
}