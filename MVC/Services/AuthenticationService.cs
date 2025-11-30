using MVC.Interfaces;
using Microsoft.AspNetCore.Http; // Necesario para IHttpContextAccessor

namespace MVC.Services
{
    // ====================================================================================
    // SERVICIO DE AUTENTICACIÓN (Lógica de Negocio de Seguridad)
    // ====================================================================================
    // CONCEPTO TEÓRICO: Servicios & State Management (TP 10 / Tema 15)
    //
    // 1. Responsabilidad: 
    //    Centralizar toda la lógica de login, logout y verificación de permisos.
    //    Los controladores solo llaman a este servicio, no manipulan la sesión directamente.
    //
    // 2. Desafío Técnico (IHttpContextAccessor):
    //    Los controladores tienen acceso directo a "HttpContext" (y Session).
    //    Pero las clases normales (como este Servicio) NO.
    //    Para acceder a la sesión aquí, necesitamos inyectar 'IHttpContextAccessor'.
    // ====================================================================================

    public class AuthenticationService : IAuthenticationService
    {
        // Dependencia para buscar usuarios en la BD.
        private readonly IUserRepository _userRepository;
        
        // Dependencia para acceder al contexto HTTP actual (Session, Request, Response).
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor con Inyección de Dependencias (DI)
        public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // --------------------------------------------------------------------------------
        // INICIAR SESIÓN (LOGIN)
        // --------------------------------------------------------------------------------
        public bool Login(string username, string password)
        {
            // 1. Accedemos al contexto actual (la petición HTTP en curso).
            var context = _httpContextAccessor.HttpContext;
            
            // Verificación de seguridad: Si no hay contexto (ej: tests), no podemos loguear.
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }

            // 2. Verificamos credenciales contra la Base de Datos.
            var user = _userRepository.GetUser(username, password);
            
            // 3. Si el usuario existe (credenciales válidas):
            if (user != null)
            {
                // 🟢 GUARDAMOS DATOS EN LA SESIÓN (State Management)
                // La sesión es un almacén de datos en el servidor vinculado al usuario por una Cookie.
                
                // Flag principal: "El usuario está logueado".
                context.Session.SetString("IsAuthenticated", "true");
                
                // Guardamos datos útiles para no tener que ir a la BD a cada rato.
                context.Session.SetString("User", user.User);        // Ej: "admin"
                context.Session.SetString("UserNombre", user.Nombre); // Ej: "Juan Perez"
                
                // ❗ ROL: Fundamental para la Autorización (HasAccessLevel).
                // Define qué puede hacer el usuario (ej: "Administrador" o "Cliente").
                context.Session.SetString("Rol", user.Rol); 
                
                return true; // Login exitoso
            }

            return false; // Credenciales inválidas
        }

        // --------------------------------------------------------------------------------
        // CERRAR SESIÓN (LOGOUT)
        // --------------------------------------------------------------------------------
        public void Logout()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }
            
            // BORRAMOS TODO: Elimina todas las claves de la sesión del usuario.
            // Esto lo desconecta efectivamente del sistema.
            context.Session.Clear();
        }

        // --------------------------------------------------------------------------------
        // VERIFICAR AUTENTICACIÓN (¿Está logueado?)
        // --------------------------------------------------------------------------------
        public bool IsAuthenticated()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }

            // Verificamos si existe la marca que pusimos en el Login.
            return context.Session.GetString("IsAuthenticated") == "true";
        }

        // --------------------------------------------------------------------------------
        // VERIFICAR AUTORIZACIÓN (¿Tiene permiso?)
        // --------------------------------------------------------------------------------
        // Este método implementa la lógica de Roles requerida en el TP 10.
        // Recibe el rol requerido (ej: "Administrador") y lo compara con el que tiene el usuario.
        public bool HasAccessLevel(string requiredAccessLevel)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HttpContext no está disponible.");
            }
            
            // Recuperamos el rol guardado en la sesión.
            string userRole = context.Session.GetString("Rol");
            
            // Comparamos. (Aquí la lógica es estricta: debe ser IGUAL).
            // En sistemas más complejos, un Admin podría tener acceso a cosas de Cliente,
            // pero para este TP, la comparación directa es suficiente.
            return userRole == requiredAccessLevel;
        }
    }
}