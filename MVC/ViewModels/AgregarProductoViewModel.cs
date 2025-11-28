using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace MVC.ViewModels;

    public class AgregarProductoViewModel
{
        public int IdPresupuesto { get; set; }

    [Display(Name = "Producto a agregar")]
        // ❗ ❗ ESTE ES EL [Required] QUE FALTA ❗ ❗
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int IdProducto { get; set; } 

        // ❗ Validación: Cantidad Requerida y Positiva
        [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }
    
    // ❗ ❗ LA SOLUCIÓN: IGNORAR ESTA PROPIEDAD EN EL POST ❗ ❗
    // Este SelectList solo se usa para renderizar el Dropdown en la vista GET.
      //  [BindNever]
        public SelectList? ListaProductos { get; set; } 
    }
