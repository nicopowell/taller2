using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.ViewModels;

namespace MVC.Controllers
{
    // ====================================================================================
    // CONTROLADOR HOME (Página de Inicio y Gestión de Errores)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Controladores MVC (TP 8)
    //
    // 1. Objetivo:
    //    Manejar las rutas generales del sitio (como la página de bienvenida) y,
    //    crucialmente, la visualización de errores no controlados.
    //
    // 2. Comportamiento por defecto:
    //    Es el controlador que se carga al entrar a la raíz "/" del sitio
    //    (definido en app.MapControllerRoute en Program.cs).
    // ====================================================================================

    public class HomeController : Controller
    {
        // ILogger: Servicio nativo de .NET Core para escribir logs (consola, archivo, nube).
        // Sirve para depurar ("Debug") o registrar fallos ("Error").
        private readonly ILogger<HomeController> _logger;

        // Inyección de Dependencias del Logger (viene configurado por defecto en el Framework).
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // --------------------------------------------------------------------------------
        // ACCIONES DE NAVEGACIÓN BÁSICA
        // --------------------------------------------------------------------------------
        
        // GET: / (Raíz) o /Home/Index
        public IActionResult Index()
        {
            // Retorna la vista Views/Home/Index.cshtml
            return View();
        }

        // GET: /Home/Privacy
        public IActionResult Privacy()
        {
            // Retorna la vista Views/Home/Privacy.cshtml
            return View();
        }

        // --------------------------------------------------------------------------------
        // MANEJO DE ERRORES GLOBALES
        // --------------------------------------------------------------------------------
        // Esta acción es invocada automáticamente por el middleware "UseExceptionHandler"
        // (ver Program.cs) cuando ocurre una excepción en cualquier parte de la app.
        
        // [ResponseCache]: Atributo que controla el caché del navegador.
        // Duration = 0, NoStore = true: Le dice al navegador "NUNCA guardes esta página".
        // Es vital para ver el error real actual y no una versión vieja cacheada.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crea un ErrorViewModel para mostrar en la vista "Error.cshtml".
            // Activity.Current?.Id ?? HttpContext.TraceIdentifier:
            // Obtiene el ID único de la solicitud (Request ID). Esto sirve para buscar
            // el error específico en los logs del servidor.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}