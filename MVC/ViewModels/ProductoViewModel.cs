using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels;

public class ProductoViewModel
{
    public ProductoViewModel()
    { 

    }

    public ProductoViewModel(Producto producto)
    {
        Descripcion = producto.Descripcion;
        IdProducto = producto.IdProducto;
        Precio = producto.Precio;
    }
    public int IdProducto { get; set; } 
    // ❗ Validación: Máximo 250 caracteres. Es opcional si no tiene [Required]
    [Display(Name = "Descripción del Producto")]
    [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
    public string Descripcion { get; set; }

    // ❗ Validación: Requerido y debe ser positivo
    [Display(Name = "Precio Unitario")]
    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")] 
    public decimal Precio { get; set; }
}
