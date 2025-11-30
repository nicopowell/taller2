using Microsoft.AspNetCore.Mvc;
using MVC.Interfaces;
using MVC.ViewModels;

namespace MVC.Controllers
{
    // ====================================================================================
    // CONTROLADOR DE LOGIN (Autenticación)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Orquestación de Seguridad (TP 10 - Fase 2)
    //
    // 1. Objetivo:
    //    Manejar el proceso de entrada (Login) y salida (Logout) del sistema.
    //    Actúa como intermediario entre la Vista (Formulario de Login) y el Servicio de Autenticación.
    //
    // 2. Responsabilidad Única:
    //    Este controlador NO contiene la lógica de verificar password o escribir en la sesión.
    //    Esa responsabilidad se delega al 'IAuthenticationService'.
    // ====================================================================================

    public class LoginController : Controller
    {
        // Dependencia Inyectada: El servicio que sabe cómo autenticar usuarios.
        private readonly IAuthenticationService _authenticationService;
        
        // Logger para registrar eventos (opcional pero recomendado para debug).
        private readonly ILogger<LoginController> _logger;

        // --------------------------------------------------------------------------------
        // CONSTRUCTOR (DI)
        // --------------------------------------------------------------------------------
        // Inyectamos el servicio de autenticación registrado en Program.cs.
        public LoginController(IAuthenticationService authenticationService, ILogger<LoginController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        // --------------------------------------------------------------------------------
        // ACCIÓN: MOSTRAR LOGIN (GET)
        // --------------------------------------------------------------------------------
        public IActionResult Index()
        {
            // Creamos un ViewModel vacío para que la vista tenga algo que renderizar.
            // Opcional: Podríamos verificar si ya está autenticado para redirigir a Home directamente.
            var model = new LoginViewModel()
            {
                // Ejemplo de cómo leer la sesión desde el controlador (aunque authService.IsAuthenticated() es mejor).
                IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true"
            };
            
            return View(model); 
        }

        // --------------------------------------------------------------------------------
        // ACCIÓN: PROCESAR LOGIN (POST)
        // --------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // 1. VALIDACIÓN DE ENTRADA (Data Annotations)
            // Verificamos que el usuario haya escrito algo en los campos (Required).
            if (!ModelState.IsValid)
            {
                // Si falta algún dato, devolvemos la vista con los mensajes de error.
                return View("Index", model);
            }

            // 2. VALIDACIÓN MANUAL (Seguridad extra)
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                model.ErrorMessage = "Por favor ingrese su nombre de usuario y contraseña.";
                return View("Index", model);
            }

            // 3. LLAMADA AL SERVICIO DE AUTENTICACIÓN (Lógica de Negocio)
            // Delegamos la verificación de credenciales al servicio.
            if (_authenticationService.Login(model.Username, model.Password))
            {
                // 🟢 ÉXITO: El servicio ya escribió en la sesión (User, Rol, etc).
                // Redirigimos al Dashboard principal.
                return RedirectToAction("Index", "Home");
            }

            // 🔴 FALLO: Credenciales incorrectas.
            // Configuramos un mensaje de error en el ViewModel para mostrar en la vista.
            model.ErrorMessage = "Credenciales inválidas.";
            model.IsAuthenticated = false;
            
            // Retornamos a la vista de login para que intente de nuevo.
            return View("Index", model);
        }

        // --------------------------------------------------------------------------------
        // ACCIÓN: CERRAR SESIÓN (LOGOUT)
        // --------------------------------------------------------------------------------
        public IActionResult Logout()
        {
            // Delegamos la limpieza de la sesión al servicio.
            _authenticationService.Logout();

            // Redirigimos al usuario a la pantalla de Login (o Home público).
            return RedirectToAction("Index");
        }
    }
}