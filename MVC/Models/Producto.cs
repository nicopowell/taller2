using System.ComponentModel.DataAnnotations;
namespace MVC.Models;

  public class Producto
    {
        [Display(Name = "ID")]
        public int IdProducto { get; set; }

        // Esto solo afecta el texto que se muestra en la cabecera de la tabla
        [Display(Name = "Descripción del Producto")] 
        public string? Descripcion { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal Precio { get; set; }
    }
