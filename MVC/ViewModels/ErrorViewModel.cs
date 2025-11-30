namespace MVC.ViewModels
{
    // ====================================================================================
    // VIEW MODEL: ERROR GENÉRICO
    // ====================================================================================
    // CONCEPTO TEÓRICO: Manejo de Errores y Diagnóstico (TP 8)
    //
    // 1. Objetivo:
    //    Transportar información técnica sobre un error no controlado a la vista 'Error.cshtml',
    //    para que el usuario (o soporte técnico) pueda identificar qué pasó.
    //
    // 2. Uso:
    //    Es instanciado por el método 'Error()' en 'HomeController' (u otros controladores)
    //    cuando ocurre una excepción capturada por el middleware de errores.
    // ====================================================================================

    public class ErrorViewModel
    {
        // Identificador único de la solicitud HTTP donde ocurrió el error.
        // Se obtiene de 'Activity.Current?.Id' o 'HttpContext.TraceIdentifier'.
        // Sirve para buscar el error específico en los logs del servidor (correlación).
        public string? RequestId { get; set; }

        // Propiedad calculada (solo lectura) que determina si se debe mostrar el ID en la vista.
        // Devuelve true si RequestId tiene algún valor.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}