using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels
{
    // ====================================================================================
    // VIEW MODEL: PRODUCTO (DTO para la Vista)
    // ====================================================================================
    // CONCEPTO TEÓRICO: ViewModels (Tema 12) & Validaciones (TP 9 - Tema 13)
    //
    // 1. ¿Qué es esto?
    //    Es una clase diseñada específicamente para transportar datos entre el Controlador y la Vista.
    //    NO es una entidad de base de datos. Su propósito es la PRESENTACIÓN y VALIDACIÓN.
    //
    // 2. Diferencias con el Modelo (Producto.cs):
    //    - Tiene validaciones estrictas ([Required], [StringLength]) para proteger la integridad de datos.
    //    - Puede tener propiedades extra que no existen en la BD (ej: confirmación de contraseña).
    //    - Puede ocultar propiedades sensibles de la BD que no queremos mostrar.
    // ====================================================================================

    public class ProductoViewModel
    {
        // --------------------------------------------------------------------------------
        // CONSTRUCTOR VACÍO (Requisito de Model Binding)
        // --------------------------------------------------------------------------------
        // ASP.NET Core necesita un constructor sin parámetros para poder instanciar esta clase
        // automáticamente cuando recibe datos de un formulario (Model Binding).
        // Si lo borras, el formulario de creación/edición fallará al enviar datos.
        public ProductoViewModel()
        { 
        }

        // --------------------------------------------------------------------------------
        // CONSTRUCTOR DE MAPEO (Modelo -> ViewModel)
        // --------------------------------------------------------------------------------
        // Este constructor facilita convertir una entidad de BD (Producto) en este ViewModel.
        // Se usa típicamente en el método GET del controlador (Edit o Details),
        // donde recuperamos datos de la BD y necesitamos mostrarlos en el formulario.
        public ProductoViewModel(Producto producto)
        {
            Descripcion = producto.Descripcion;
            IdProducto = producto.IdProducto;
            Precio = producto.Precio;
        }

        // --------------------------------------------------------------------------------
        // PROPIEDADES Y VALIDACIONES (Data Annotations)
        // --------------------------------------------------------------------------------

        // ID del producto. Necesario para identificar qué registro estamos editando.
        // En 'Create', suele ser 0 o ignorado.
        public int IdProducto { get; set; } 

        // --- VALIDACIÓN DE DESCRIPCIÓN ---
        // 1. [Display]: Define qué texto se muestra en el <label> del HTML (Tag Helper asp-for).
        // 2. [StringLength]: Regla de negocio (TP 9). Limita la longitud para evitar errores de BD 
        //    o abusos. 'ErrorMessage' personaliza el texto que ve el usuario si falla.
        // NOTA: Al no tener [Required], este campo es opcional (puede dejarse vacío).
        [Display(Name = "Descripción del Producto")]
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string Descripcion { get; set; }

        // --- VALIDACIÓN DE PRECIO ---
        // 1. [Required]: Hace que el campo sea OBLIGATORIO. Si el usuario lo deja vacío,
        //    'ModelState.IsValid' será false y se mostrará el error.
        // 2. [Range]: Regla de negocio (TP 9). Impide precios negativos o cero.
        //    0.01 es el mínimo, double.MaxValue es el máximo técnico posible.
        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")] 
        public decimal Precio { get; set; }
    }
}