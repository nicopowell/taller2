namespace MVC.Models
{
    // ====================================================================================
    // MODELO DE DOMINIO: USUARIO
    // ====================================================================================
    // CONCEPTO TEÓRICO: Entidad de Dominio (TP 10 - Seguridad)
    // 
    // 1. Objetivo: Representar al usuario del sistema en la base de datos y en la lógica.
    //    Esta clase mapea directamente a la tabla "Usuarios" de la BD.
    //
    // 2. Roles: El campo 'Rol' es fundamental para la Autorización (decidir qué permisos tiene).
    // ====================================================================================

    public class Usuario
    {
        // Identificador único (PK).
        public int Id { get; set; }
        
        // Nombre real de la persona (ej: "Juan Pérez").
        // El '?' indica que puede ser nulo en la base de datos (Nullable).
        public string? Nombre { get; set; }
        
        // Nombre de usuario para el login (ej: "juanp").
        public string? User{ get; set; }
        
        // Contraseña.
        // NOTA DE SEGURIDAD: En un sistema real, NUNCA guardar contraseñas en texto plano.
        // Se debería guardar un Hash (ej: BCrypt, SHA256 con sal).
        // Para este TP académico, se suele aceptar texto plano por simplicidad.
        public string? Pass { get; set; } 
        
        // Rol del usuario (ej: "Administrador", "Cliente").
        // Este valor se guardará en la Sesión al hacer Login y se usará en
        // los controladores para restringir el acceso (Autorización).
        public string? Rol { get; set; } 
    }
}