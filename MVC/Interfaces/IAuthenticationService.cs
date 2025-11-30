namespace MVC.Interfaces
{
    // ====================================================================================
    // INTERFAZ DE SERVICIO DE AUTENTICACIÓN
    // ====================================================================================
    // CONCEPTO TEÓRICO: Abstracción de Servicios (TP 10 - Fase 1)
    //
    // 1. Objetivo: 
    //    Definir QUÉ operaciones de seguridad puede realizar la aplicación, sin exponer
    //    CÓMO se realizan (ej: si usa Cookies, JWT, Session, o Auth de Google).
    //
    // 2. Desacoplamiento:
    //    Los controladores (Login, Productos, Presupuestos) dependen de ESTA interfaz,
    //    no de la clase concreta 'AuthenticationService'. Esto permite cambiar la
    //    implementación de seguridad en el futuro sin tocar los controladores.
    // ====================================================================================

    public interface IAuthenticationService
    {
        // --------------------------------------------------------------------------------
        // GESTIÓN DE SESIÓN (LOGIN/LOGOUT)
        // --------------------------------------------------------------------------------
        
        // Valida las credenciales del usuario y, si son correctas, crea la sesión.
        // Retorna true si el login fue exitoso, false si falló.
        bool Login(string username, string password);

        // Cierra la sesión actual y limpia los datos del usuario.
        void Logout();

        // --------------------------------------------------------------------------------
        // VERIFICACIÓN DE ESTADO (AUTENTICACIÓN)
        // --------------------------------------------------------------------------------
        
        // Verifica si hay un usuario logueado actualmente.
        // Se usa en los controladores para proteger rutas (si devuelve false -> redirigir a Login).
        bool IsAuthenticated();

        // --------------------------------------------------------------------------------
        // VERIFICACIÓN DE PERMISOS (AUTORIZACIÓN)
        // --------------------------------------------------------------------------------
        
        // Verifica si el usuario actual tiene un Rol específico.
        // requiredAccessLevel: El rol necesario (ej: "Administrador").
        // Se usa para proteger acciones críticas (ej: Crear/Borrar productos).
        bool HasAccessLevel(string requiredAccessLevel);
    }
}