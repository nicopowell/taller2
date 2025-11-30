using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    // ====================================================================================
    // MODELO DE DOMINIO (ENTIDAD)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Entidades (Tema 12 - Arquitecturas Limpias) & ADO.NET (Tema 08)
    //
    // 1. ¿Qué es esto?
    //    Es una clase POCO (Plain Old CLR Object). Representa una "Entidad" del negocio.
    //    En este proyecto, representa una FILA de la tabla "Productos" en la base de datos.
    //
    // 2. Diferencia con ViewModel (TP 9):
    //    - Modelo (Este archivo): Es la estructura REAL de la base de datos.
    //    - ViewModel (ProductoViewModel): Es la estructura que necesita la VISTA (el formulario).
    //      El ViewModel suele tener validaciones ([Required]) que aquí a veces no se ponen
    //      para mantener el modelo "puro" o alineado solo con la DB.
    // ====================================================================================

    public class Producto
    {
        // --------------------------------------------------------------------------------
        // CLAVE PRIMARIA (PK)
        // --------------------------------------------------------------------------------
        // Mapea con la columna "IdProducto" (INTEGER PRIMARY KEY) en SQLite.
        // El atributo [Display] cambia cómo se ve el nombre del campo en el HTML (Label).
        [Display(Name = "ID")]
        public int IdProducto { get; set; }

        // --------------------------------------------------------------------------------
        // CAMPOS DE DATOS
        // --------------------------------------------------------------------------------
        // Mapea con la columna "Descripcion" (TEXT) en SQLite.
        // El signo de pregunta 'string?' indica que admite nulos (Nullable),
        // aunque la regla de negocio del TP 9 luego lo haga obligatorio en el ViewModel.
        
        [Display(Name = "Descripción del Producto")] 
        public string? Descripcion { get; set; }

        // Mapea con la columna "Precio" (REAL/NUMERIC) en SQLite.
        // Se usa 'decimal' en C# para evitar errores de redondeo con dinero.
        
        [Display(Name = "Precio Unitario")]
        public decimal Precio { get; set; }
    }
}