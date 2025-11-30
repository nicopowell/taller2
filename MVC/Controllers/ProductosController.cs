using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Repositorios;
using MVC.ViewModels; 
using MVC.Interfaces;

namespace MVC.Controllers
{
    // ====================================================================================
    // CONTROLADOR DE PRODUCTOS
    // ====================================================================================
    // CONCEPTO TEÓRICO: Controlador (MVC - Tema 10) & Inyección de Dependencias (TP 10)
    //
    // 1. ¿Qué hace? 
    //    Es el "Director de Orquesta". Recibe las peticiones del usuario (Browser),
    //    habla con el Modelo/Repositorio para obtener/guardar datos, y elige qué Vista mostrar.
    //
    // 2. Inyección de Dependencias (DI):
    //    No crea sus propias dependencias (no hace 'new ProductoRepository()').
    //    Las pide en el constructor y el contenedor de servicios (Program.cs) se las da.
    // ====================================================================================

    public class ProductosController : Controller
    {
        // Definimos las variables privadas para almacenar los servicios inyectados.
        // Usamos las INTERFACES, no las clases concretas, para mantener el desacoplamiento.
        private IProductoRepository _repo; 
        private IAuthenticationService _authService;

        // --------------------------------------------------------------------------------
        // CONSTRUCTOR (INYECCIÓN DE DEPENDENCIAS)
        // --------------------------------------------------------------------------------
        // Cuando ASP.NET necesita este controlador, mira este constructor.
        // Ve que necesita IProductoRepository y IAuthenticationService.
        // Busca en 'Program.cs' quiénes implementan esas interfaces (AddScoped) y los inyecta.
        public ProductosController(IProductoRepository prodRepo, IAuthenticationService authService)
        {
            // _repo = new ProductoRepository(); // <- ESTO SERÍA ACOPLAMIENTO (MALO en TP 10)
            
            // Asignamos las instancias que nos dio el contenedor de DI.
            _repo = prodRepo;
            _authService = authService;
        }

        // --------------------------------------------------------------------------------
        // LECTURA: LISTADO DE PRODUCTOS (INDEX)
        // --------------------------------------------------------------------------------
        public IActionResult Index()
        {
            // 1. SEGURIDAD (TP 10): Verificar si el usuario puede ver esto.
            // Si devuelve algo distinto de null, es una redirección (Login o Error).
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            // 2. LÓGICA: Pedir datos al repositorio.
            List<Producto> productos = _repo.GetAll(); 

            // 3. VISTA: Entregar los datos a la vista 'Index.cshtml'.
            return View(productos);
        }

        // --------------------------------------------------------------------------------
        // LECTURA: DETALLE DE UN PRODUCTO
        // --------------------------------------------------------------------------------
        public IActionResult Details(int id)
        {
            // Seguridad: Solo admins.
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto producto = _repo.GetById(id);
            
            // Si no existe el ID buscado, devolvemos un error 404 estandar.
            if (producto == null) return NotFound();
            
            return View(producto);
        }

        // --------------------------------------------------------------------------------
        // CREACIÓN (GET): MOSTRAR FORMULARIO VACÍO
        // --------------------------------------------------------------------------------
        public IActionResult Create()
        {
            // Seguridad: Solo admins.
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;
            
            // Retorna la vista vacía para que el usuario llene los datos.
            return View();
        }

        /* VERSIÓN ANTIGUA (TP 8 - SIN VIEWMODEL)
           Esta versión recibía directamente la entidad 'Producto'.
           Se deja comentada para mostrar la evolución hacia ViewModels en TP 9.
        
           [HttpPost]
           public IActionResult Create(Producto producto)
           {
               _repo.Add(producto);
               return RedirectToAction(nameof(Index));
           }
        */
 
        // --------------------------------------------------------------------------------
        // CREACIÓN (POST): PROCESAR DATOS DEL FORMULARIO
        // --------------------------------------------------------------------------------
        // Recibe un ProductoViewModel, NO un Producto (TP 9).
        // Esto permite validar los datos de entrada antes de tocar el dominio.
        [HttpPost]
        public IActionResult Create(ProductoViewModel productoVM) 
        {
            // ❗ 1. CHEQUEO DE SEGURIDAD DE DATOS (TP 9)
            // Verifica si se cumplen los Data Annotations ([Required], [Range]) del ViewModel.
            if (!ModelState.IsValid)
            {
                // ❌ FALLÓ LA VALIDACIÓN: 
                // Retornamos la misma vista con el objeto VM cargado.
                // Esto permite mostrar los mensajes de error y no borrar lo que el usuario escribió.
                return View(productoVM); 
            }
            
            // 🟢 2. MAPEO (ViewModel -> Modelo de Dominio)
            // Convertimos el DTO (Data Transfer Object) a la Entidad real que la BD entiende.
            var nuevoProducto = new Producto
            {
                Descripcion = productoVM.Descripcion,
                Precio = productoVM.Precio 
                // IdProducto no se asigna porque es Autoincremental en Create.
            };

            // 3. PERSISTENCIA: Mandamos la entidad pura al repositorio.
            _repo.Add(nuevoProducto); 

            // 4. REDIRECCIÓN (Patrón PRG: Post-Redirect-Get)
            // Evita que si el usuario recarga la página, se envíe el formulario de nuevo (duplicados).
            return RedirectToAction(nameof(Index)); 
        }

        // --------------------------------------------------------------------------------
        // EDICIÓN (GET): MOSTRAR FORMULARIO CON DATOS CARGADOS
        // --------------------------------------------------------------------------------
        public IActionResult Edit(int id)
        {
            // Seguridad: Solo admins.
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            // 1. Buscamos el producto real en la BD.
            Producto producto = _repo.GetById(id);
            
            if (producto == null)
            {
                return NotFound();
            }

            // 2. MAPEO INVERSO (Modelo -> ViewModel)
            // Necesitamos pasarle un ViewModel a la vista, así que convertimos la entidad
            // usando el constructor auxiliar que creamos en ProductoViewModel.
            ProductoViewModel productovm = new ProductoViewModel(producto);
            
            return View(productovm);
        }

        /* VERSIÓN ANTIGUA (TP 8 - SIN VIEWMODEL)
           [HttpPost]
           public IActionResult Edit(int id, Producto producto)
           {
               if (id != producto.IdProducto) return NotFound();
               _repo.Update(producto);
               return RedirectToAction(nameof(Index));
           }
        */

        // --------------------------------------------------------------------------------
        // EDICIÓN (POST): GUARDAR CAMBIOS
        // --------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult Edit(int id, ProductoViewModel productoVM)
        {
            // Seguridad básica: Asegurar que el ID de la URL coincida con el del formulario.
            if (id != productoVM.IdProducto) return NotFound();

            // ❗ 1. CHEQUEO DE SEGURIDAD (Igual que en Create)
            if (!ModelState.IsValid)
            {
                return View(productoVM); 
            }

            // 🟢 2. MAPEO (ViewModel -> Modelo)
            var productoAEditar = new Producto
            {
                IdProducto = productoVM.IdProducto, // Aquí SÍ necesitamos el ID para el WHERE SQL.
                Descripcion = productoVM.Descripcion,
                Precio = productoVM.Precio
            };

            // 3. PERSISTENCIA
            _repo.Update(productoAEditar);
            
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------------------
        // ELIMINACIÓN (GET): PANTALLA DE CONFIRMACIÓN
        // --------------------------------------------------------------------------------
        public IActionResult Delete(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto producto = _repo.GetById(id);
            if (producto == null) return NotFound();
            
            return View(producto);
        }

        // --------------------------------------------------------------------------------
        // ELIMINACIÓN (POST): BORRAR DEFINITIVAMENTE
        // --------------------------------------------------------------------------------
        // ActionName("Delete") permite que la URL sea /Delete/5 aunque el método se llame distinto.
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id); 
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------------------
        // MANEJO DE ERRORES (GENÉRICO)
        // --------------------------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ================================================================================
        // MÉTODOS PRIVADOS DE AYUDA (HELPER METHODS)
        // ================================================================================

        // --------------------------------------------------------------------------------
        // VALIDACIÓN DE SEGURIDAD CENTRALIZADA (TP 10 - Autorización Manual)
        // --------------------------------------------------------------------------------
        // Este método implementa la lógica de autorización requerida en el TP 10.
        // Verifica Autenticación (¿Quién sos?) y Autorización (¿Qué rol tenés?).
        private IActionResult CheckAdminPermissions()
        {
            // 1. Autenticación: ¿El usuario inició sesión?
            if (!_authService.IsAuthenticated())
            {
                // Si no, redirigir al Login para que ingrese.
                return RedirectToAction("Index", "Login");
            }
            
            // 2. Autorización: ¿El usuario es Administrador?
            // (Regla de Negocio TP 10: Solo Admins pueden gestionar Productos).
            if (!_authService.HasAccessLevel("Administrador"))
            {
                // Si está logueado pero no es admin (ej: Cliente), redirigir a "Acceso Denegado".
                return RedirectToAction(nameof(AccesoDenegado)); 
            }
            
            // Si pasa ambos chequeos, devuelve null (luz verde para continuar).
            return null; 
        }

        public IActionResult AccesoDenegado()
        {
            // Muestra una vista explicando que no tiene permisos suficientes.
            return View();
        }   
    }
}