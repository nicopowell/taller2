using System.ComponentModel.DataAnnotations;
using System;
using MVC.Models;

namespace MVC.ViewModels
{
    // ====================================================================================
    // VIEW MODEL: PRESUPUESTO (DTO para Vistas de Creación/Edición)
    // ====================================================================================
    // CONCEPTO TEÓRICO: ViewModels (TP 9/12) & Validación de Datos
    //
    // 1. Objetivo:
    //    Representar los datos necesarios para crear o editar un presupuesto en la UI.
    //    A diferencia del modelo 'Presupuesto', este NO incluye la lista de detalles
    //    porque el alta de detalles se hace en un paso posterior o separado.
    //
    // 2. Validaciones:
    //    Implementa las reglas de negocio de entrada de datos (Required, DataType).
    // ====================================================================================

    public class PresupuestoViewModel
    {
        // --------------------------------------------------------------------------------
        // CONSTRUCTOR VACÍO (Requisito de Model Binding)
        // --------------------------------------------------------------------------------
        // Necesario para que ASP.NET pueda crear una instancia de esta clase cuando
        // recibe los datos del formulario HTTP POST.
        public PresupuestoViewModel()
        {
        }

        // --------------------------------------------------------------------------------
        // CONSTRUCTOR DE MAPEO (Modelo -> ViewModel)
        // --------------------------------------------------------------------------------
        // Facilita la conversión de una entidad de BD a este DTO.
        // Usado en el método GET de Edit() para precargar los datos existentes.
        public PresupuestoViewModel(Presupuesto presupuesto)
        {
            IdPresupuesto = presupuesto.IdPresupuesto;
            NombreDestinatario = presupuesto.NombreDestinatario;
            FechaCreacion = presupuesto.FechaCreacion;
        }

        // Identificador único (necesario para Edit/Delete).
        public int IdPresupuesto { get; set; }

        // --------------------------------------------------------------------------------
        // VALIDACIÓN DE DESTINATARIO
        // --------------------------------------------------------------------------------
        // TP 9 Requisito: Nombre Destinatario debe ser obligatorio.
        // [Display]: Configura el texto del <label> en el HTML.
        // [Required]: Si el campo está vacío o es null, ModelState.IsValid será false.
        
        // NOTA: Si en el futuro se pidiera email, se descomentaría [EmailAddress].
        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        // [EmailAddress(ErrorMessage = "El formato de email no es válido.")] 
        public string NombreDestinatario { get; set; } 
        
        // --------------------------------------------------------------------------------
        // VALIDACIÓN DE FECHA
        // --------------------------------------------------------------------------------
        // TP 9 Requisito: Fecha obligatoria.
        // [DataType(DataType.Date)]:
        // 1. Formatea el valor para mostrar solo fecha (sin hora).
        // 2. Genera un input HTML5 de tipo <input type="date">, habilitando el calendario nativo.
        
        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)] 
        public DateTime FechaCreacion { get; set; } 
    }
}