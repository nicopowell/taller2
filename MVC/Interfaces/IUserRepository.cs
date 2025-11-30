using MVC.Models;

namespace MVC.Interfaces
{
    // ====================================================================================
    // INTERFAZ DE REPOSITORIO DE USUARIOS
    // ====================================================================================
    // CONCEPTO TEÓRICO: Abstracción de Datos (TP 10)
    // 
    // 1. Objetivo: Desacoplar la lógica de autenticación (Service) del acceso a datos (SQL).
    //    El AuthenticationService no debe saber si los usuarios vienen de SQL, un archivo de texto o Google.
    //    Solo debe saber que existe un método 'GetUser' que le devuelve un usuario o null.
    //
    // 2. Inyección de Dependencias:
    //    Esta interfaz se inyecta en 'AuthenticationService' y se implementa en 'UsuarioRepository'.
    // ====================================================================================

    public interface IUserRepository
    {
        // Busca un usuario en la fuente de datos (BD) validando sus credenciales.
        // username: Nombre de usuario (ej: "admin").
        // password: Contraseña en texto plano (en un sistema real debería estar hasheada).
        // Retorno: Objeto Usuario lleno si las credenciales son correctas, o NULL si fallan.
        Usuario GetUser(string username, string password);
    }
}