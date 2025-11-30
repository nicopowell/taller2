using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    // ====================================================================================
    // CONTROLADOR DE ERRORES (Manejo de Excepciones y Accesos Denegados)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Manejo de Errores y UX (TP 10)
    //
    // 1. Propósito:
    //    Centralizar la lógica de visualización de errores. En lugar de mostrar
    //    una pantalla blanca o un stack trace técnico, mostramos una vista amigable.
    //
    // 2. Seguridad:
    //    Es importante no revelar detalles técnicos del error al usuario final.
    // ====================================================================================

    public class ErrorController : Controller
    {
        // --------------------------------------------------------------------------------
        // ERROR 403 - FORBIDDEN (Acceso Denegado)
        // --------------------------------------------------------------------------------
        // Esta acción se invoca cuando un usuario logueado intenta entrar a una zona prohibida
        // para su rol (ej: un Cliente queriendo entrar a /Productos/Create).
        
        // La redirección suele hacerse desde otros controladores con:
        // return RedirectToAction("Error403", "Error");
        public IActionResult Error403()
        {
            // Devuelve la vista ubicada en Views/Error/403.cshtml (o Views/Shared/Error403.cshtml)
            // Es importante que esa vista exista.
            return View(); 
        }
        
        // Puedes agregar más métodos para otros errores comunes (404, 500) aquí.
    }
}