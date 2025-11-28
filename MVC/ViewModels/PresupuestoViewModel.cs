using System.ComponentModel.DataAnnotations;
using System;
using MVC.Models;

namespace MVC.ViewModels;

    public class PresupuestoViewModel
    {
    public PresupuestoViewModel()
    {
    }

    public PresupuestoViewModel(Presupuesto presupuesto)
    {
        IdPresupuesto = presupuesto.IdPresupuesto;
        NombreDestinatario = presupuesto.NombreDestinatario;
        FechaCreacion = presupuesto.FechaCreacion;
    }

    public int IdPresupuesto { get; set; }

        // ❗ Validación: Requerido. Se usa [EmailAddress] si se opta por guardar el mail.
        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        // [EmailAddress(ErrorMessage = "El formato de email no es válido.")] // Usar si se exige email
        public string NombreDestinatario { get; set; } 
        
        // ❗ Validación: Requerido y tipo de dato
        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)] 
        public DateTime FechaCreacion { get; set; } 
    }
