using Microsoft.AspNetCore.Mvc;
using MVC.Interfaces;
using MVC.ViewModels;
public class LoginController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<LoginController> _logger;

    // Inyección de dependencias
    public LoginController(IAuthenticationService authenticationService, ILogger<LoginController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    // Acción que devuelve la vista de login
    public IActionResult Index()
    {
        // Crear un nuevo ViewModel, pasamos el estado de autenticación
        var model = new LoginViewModel()
        {
            IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true"
        };
        return View(model); // Pasamos el ViewModel con la propiedad de autenticación
    }

    // Acción para procesar el login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        // Verificar que los datos de entrada no estén vacíos
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            model.ErrorMessage = "Por favor ingrese su nombre de usuario y contraseña.";
            return View("Index", model);
        }

        // Si el usuario existe y las credenciales son correctas
        if (_authenticationService.Login(model.Username, model.Password))
        {
            // Redirigir a la página principal o dashboard
            return RedirectToAction("Index", "Home");
        }

        // Si las credenciales no son correctas, mostrar mensaje de error
        model.ErrorMessage = "Credenciales inválidas.";
        model.IsAuthenticated = false;
        return View("Index", model);
    }

    // Acción para cerrar sesión
    public IActionResult Logout()
    {
        // Limpiar la sesión
        _authenticationService.Logout();

        // Redirigir a la vista de login
        return RedirectToAction("Index");
    }
}

