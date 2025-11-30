using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    // ====================================================================================
    // VIEW MODEL: LOGIN (Autenticación)
    // ====================================================================================
    // CONCEPTO TEÓRICO: ViewModels & Autenticación (TP 10)
    //
    // 1. Objetivo:
    //    Capturar las credenciales (Usuario/Contraseña) ingresadas por el usuario en la vista de Login.
    //    Este objeto NO se guarda en la base de datos, es efímero (solo para la petición POST).
    //
    // 2. Seguridad:
    //    Usamos DataAnnotations para validar que los campos no estén vacíos antes de procesarlos.
    // ====================================================================================

    public class LoginViewModel
    {
        // --------------------------------------------------------------------------------
        // NOMBRE DE USUARIO
        // --------------------------------------------------------------------------------
        // [Required]: No permitimos enviar el formulario si este campo está vacío.
        // El signo '?' indica que la propiedad puede ser nula inicialmente (Nullable Reference Types).
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string? Username { get; set; }

        // --------------------------------------------------------------------------------
        // CONTRASEÑA
        // --------------------------------------------------------------------------------
        // [DataType(DataType.Password)]: 
        // 1. Renderiza un <input type="password"> en HTML para que no se vean los caracteres.
        // 2. Es puramente visual, no encripta el dato en el envío (para eso se necesita HTTPS).
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // --------------------------------------------------------------------------------
        // MENSAJE DE ERROR
        // --------------------------------------------------------------------------------
        // Esta propiedad se usa para devolver feedback al usuario si el Login falla 
        // (ej: "Usuario o contraseña incorrectos").
        // El controlador llenará esta propiedad antes de retornar la vista.
        public string? ErrorMessage { get; set; }

        // --------------------------------------------------------------------------------
        // ESTADO DE AUTENTICACIÓN (Opcional/Auxiliar)
        // --------------------------------------------------------------------------------
        // Puede usarse para ocultar/mostrar elementos en la vista de Login dependiendo
        // de si el usuario ya intentó loguearse o si ya tiene sesión activa.
        public bool? IsAuthenticated { get; set; } 
    }
}