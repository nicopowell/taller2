using MVC.Interfaces; 
using MVC.Repositorios; 
using MVC.Services; 

// ====================================================================================
// 1. INICIALIZACIÓN DEL BUILDER
// ====================================================================================
// WebApplication.CreateBuilder inicializa una nueva instancia de la aplicación web.
// Configura valores predeterminados (Kestrel server, configuración de appsettings.json, etc.).
var builder = WebApplication.CreateBuilder(args);

// ====================================================================================
// 2. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DE INYECCIÓN DE DEPENDENCIAS)
// Aquí registramos todas las dependencias que nuestra aplicación necesita.
// ASP.NET Core se encargará de crear y destruir estas instancias según sea necesario.
// ====================================================================================

// --- A. CONFIGURACIÓN DE SESIONES (TP 10 - Estado y Autenticación) ---
// Habilitamos el servicio de sesiones para poder persistir datos del usuario (como Login y Rol)
// entre diferentes peticiones HTTP (ya que HTTP es un protocolo sin estado "Stateless").
builder.Services.AddSession(options =>
{
    // IdleTimeout: Tiempo que la sesión permanece activa sin interacción del usuario.
    // Si el usuario no hace nada por 30 min, la sesión muere y se pierden los datos.
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    
    // HttpOnly = true: Medida de SEGURIDAD. Impide que scripts del lado del cliente (JS)
    // accedan a la cookie de sesión, protegiendo contra ataques XSS (Cross-Site Scripting).
    options.Cookie.HttpOnly = true; 
    
    // IsEssential = true: Indica que esta cookie es fundamental para que la app funcione.
    // Esto es importante para cumplir con leyes de privacidad (como GDPR), permitiendo
    // que la cookie se cree incluso si el usuario no ha aceptado el consentimiento de rastreo.
    options.Cookie.IsEssential = true; 
});

// --- B. ACCESO AL CONTEXTO HTTP (TP 10 - Inyección en Servicios) ---
// IHttpContextAccessor permite acceder a 'HttpContext' (y por ende a la Sesión) 
// desde clases que NO son Controladores (como nuestro 'AuthenticationService').
// Sin esto, no podríamos leer la sesión dentro de la capa de Servicios.
builder.Services.AddHttpContextAccessor();

// --- C. REGISTRO DE INYECCIÓN DE DEPENDENCIAS (TP 10 - Patrón Repositorio y DIP) ---
// Aplicamos el Principio de Inversión de Dependencias (la 'D' de SOLID).
// Sintaxis: builder.Services.AddScoped<INTERFAZ, CLASE_CONCRETA>();
// Significado: "Cuando un constructor pida 'IUserRepository', entrégale una instancia de 'UsuarioRepository'".

// AddScoped (Ciclo de Vida):
// Se crea una instancia NUEVA por cada Petición HTTP (Request) y se reutiliza dentro de esa misma petición.
// Es el ciclo de vida ideal para Repositorios de Base de Datos y Servicios de Negocio,
// ya que asegura que la conexión a la BD se abre y cierra correctamente por cada request.

builder.Services.AddScoped<IUserRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();

// --- D. SERVICIOS MVC (TP 8) ---
// Agrega los servicios necesarios para que funcionen los Controladores y las Vistas con Razor.
builder.Services.AddControllersWithViews();

// ====================================================================================
// 3. CONSTRUCCIÓN DE LA APLICACIÓN
// ====================================================================================
var app = builder.Build();

// ====================================================================================
// 4. PIPELINE DE MIDDLEWARES (EL ORDEN ES CRÍTICO)
// Aquí definimos cómo se procesa cada petición HTTP que entra al servidor.
// Un middleware pasa la petición al siguiente. El orden altera el comportamiento.
// ====================================================================================

// --- E. ACTIVACIÓN DE SESIONES ---
// IMPORTANTE: app.UseSession() debe ir ANTES de app.UseRouting() y app.UseAuthorization().
// Razón: El sistema necesita cargar los datos de la sesión (quién es el usuario) 
// ANTES de intentar decidir a qué ruta va o si tiene permisos para verla.
app.UseSession();

// Configuración para manejo de errores según el entorno.
if (!app.Environment.IsDevelopment())
{
    // En Producción: Muestra una vista de error amigable.
    app.UseExceptionHandler("/Home/Error");
    // HSTS: Fuerza al navegador a usar siempre HTTPS (Seguridad estricta).
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirige automáticamente peticiones HTTP a HTTPS.

// Permite servir archivos estáticos (CSS, JS, Imágenes) desde la carpeta wwwroot.
// Sin esto, no cargarían los estilos de Bootstrap ni los scripts.
app.UseStaticFiles();

// Habilita el sistema de enrutamiento (decidir qué Controlador/Acción ejecutar según la URL).
app.UseRouting();

// Habilita la autorización. Aunque en este TP hacemos validaciones manuales en el controlador,
// este middleware es necesario para que el atributo [Authorize] funcione si decidiéramos usarlo.
app.UseAuthorization();

// --- F. MAPEO DE RUTAS POR DEFECTO (Convención MVC) ---
// Define el patrón de URL: dominio.com/{Controlador}/{Accion}/{Id opcional}
// Si el usuario entra a la raíz "/", por defecto irá a "HomeController", acción "Index".
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecuta la aplicación y se queda escuchando peticiones.
app.Run();