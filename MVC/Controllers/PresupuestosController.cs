using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC.Models; 
using MVC.ViewModels; 
using Microsoft.AspNetCore.Mvc.Rendering; 
using MVC.Interfaces;

namespace MVC.Controllers
{
    // ====================================================================================
    // CONTROLADOR DE PRESUPUESTOS
    // ====================================================================================
    // CONCEPTO TEÓRICO: Orquestación Compleja (TP 8/9/10)
    //
    // 1. Responsabilidad: Gestionar el ciclo de vida de los Presupuestos Y sus detalles.
    // 2. Dependencias:
    //    - IPresupuestoRepository: Para guardar/leer presupuestos.
    //    - IProductoRepository: Para llenar los dropdowns de productos.
    //    - IAuthenticationService: Para proteger las rutas (Seguridad).
    // ====================================================================================

    public class PresupuestosController : Controller
    {
        // Variables privadas para las dependencias inyectadas (Interfaces).
        private IPresupuestoRepository _repo; 
        private IProductoRepository _productoRepo; 
        private IAuthenticationService _authService;

        // --------------------------------------------------------------------------------
        // CONSTRUCTOR (DI)
        // --------------------------------------------------------------------------------
        // Recibe 3 servicios inyectados por el contenedor de dependencias (Program.cs).
        public PresupuestosController(IPresupuestoRepository repo, IProductoRepository prodRepo, IAuthenticationService authService)
        {
            _repo = repo;
            _productoRepo = prodRepo;
            _authService = authService;
        }

        // --------------------------------------------------------------------------------
        // 1. LISTAR (INDEX)
        // --------------------------------------------------------------------------------
        public IActionResult Index()
        {
            // 1. AUTENTICACIÓN: ¿Está logueado?
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            // 2. AUTORIZACIÓN: Regla de Negocio TP 10
            // "Administradores" y "Clientes" pueden ver el listado.
            if (_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente"))
            {
                // Si tiene permiso, buscamos los datos y mostramos la vista.
                var presupuestos = _repo.GetAll();
                return View(presupuestos);
            }
            else
            {
                // Usuario logueado pero sin rol válido (ej: rol desconocido).
                return RedirectToAction("Index", "Login");
            }
        }

        // --------------------------------------------------------------------------------
        // 2. DETALLE (DETAILS)
        // --------------------------------------------------------------------------------
        // Muestra la cabecera del presupuesto Y la lista de productos asociados.
        public IActionResult Details(int id)
        {
            // Reutilizamos la lógica de seguridad del Index (lectura permitida a ambos).
            // Nota: Podría refactorizarse en un método privado 'CheckReadPermissions' para ser más DRY.
            
            // Carga "Eager" (Ansiosa): GetById trae el presupuesto CON sus detalles.
            var presupuesto = _repo.GetById(id);
            
            if (presupuesto == null)
            {
                return NotFound();
            }
            
            return View(presupuesto); 
        }

        // --------------------------------------------------------------------------------
        // 3. CREAR (CREATE) - Solo Cabecera
        // --------------------------------------------------------------------------------
        // GET: Muestra el formulario vacío.
        public IActionResult Create()
        {
            // 1. Autenticación
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            // 2. Autorización ESTRICTA: Solo Administradores pueden crear.
            if (!_authService.HasAccessLevel("Administrador"))
            {
                return RedirectToAction(nameof(AccesoDenegado));
            }
            
            // Retorna un VM vacío para que los Tag Helpers generen el formulario.
            return View(new PresupuestoViewModel());
        }

        // POST: Procesa la creación de la cabecera.
        [HttpPost]
        public IActionResult Create(PresupuestoViewModel presupuestoVM) 
        {
            // ❗ 1. VALIDACIÓN PERSONALIZADA (Regla de Negocio TP 9)
            // "La fecha de creación no puede ser futura".
            // Validamos esto manualmente y agregamos el error al ModelState si falla.
            if (presupuestoVM.FechaCreacion > DateTime.Today)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser una fecha futura.");
            }
            
            // ❗ 2. CHEQUEO DE SEGURIDAD GENERAL
            if (!ModelState.IsValid)
            {
                // Si falla, volvemos a la vista mostrando los errores.
                return View(presupuestoVM); 
            }
            
            // 🟢 3. MAPEO (VM -> Entidad)
            var nuevoPresupuesto = new Presupuesto
            {
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = presupuestoVM.FechaCreacion
            };

            // 4. PERSISTENCIA
            _repo.Add(nuevoPresupuesto); 
            return RedirectToAction(nameof(Index)); 
        }

        // --------------------------------------------------------------------------------
        // 4. EDITAR (EDIT) - Solo Cabecera
        // --------------------------------------------------------------------------------
        // GET: Carga datos para editar.
        public IActionResult Edit(int id)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");

            // Solo Admin puede editar.
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            
            var presupuesto = _repo.GetById(id);
            if (presupuesto == null) return NotFound();

            // Mapeo Inverso: Entidad -> VM para mostrar en la vista.
            var presupuestoVM = new PresupuestoViewModel(presupuesto);

            return View(presupuestoVM);
        }

        // POST: Guarda cambios.
        [HttpPost]
        public IActionResult Edit(int id, PresupuestoViewModel presupuestoVM)
        {
            if (id != presupuestoVM.IdPresupuesto) return NotFound();

            // 1. Validación de Regla de Negocio (Fecha).
            if (presupuestoVM.FechaCreacion > DateTime.Today)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser una fecha futura.");
            }

            // 2. Validación del Modelo.
            if (!ModelState.IsValid)
            {
                return View(presupuestoVM); 
            }

            // 3. Mapeo.
            var presupuestoAEditar = new Presupuesto
            {
                IdPresupuesto = presupuestoVM.IdPresupuesto,
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = presupuestoVM.FechaCreacion
            };

            // 4. Update.
            _repo.Update(presupuestoAEditar);
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------------------
        // 5. ELIMINAR (DELETE)
        // --------------------------------------------------------------------------------
        public IActionResult Delete(int id)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));

            var presupuesto = _repo.GetById(id);
            if (presupuesto == null) return NotFound();
            
            return View(presupuesto);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // El repositorio se encarga de borrar primero los detalles y luego la cabecera.
            _repo.Delete(id); 
            return RedirectToAction(nameof(Index));
        }

        // ====================================================================================
        // 6. LÓGICA RELACIONAL: AGREGAR PRODUCTO (N:M) - ¡IMPORTANTE!
        // ====================================================================================
        // Esta sección maneja la complejidad de agregar un ítem a un presupuesto existente.
        
        // GET: Muestra el formulario con el Dropdown de productos.
        public IActionResult AgregarProducto(int id)
        {
            if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            
            // 1. Obtener lista de productos para el <select>.
            List<Producto> productos = _productoRepo.GetAll();
            
            // 2. Configurar el ViewModel especial para esta acción.
            AgregarProductoViewModel model = new AgregarProductoViewModel
            {
                IdPresupuesto = id, // Guardamos a qué presupuesto volveremos.
                // SelectList(FuenteDatos, ValorOculto, TextoVisible)
                ListaProductos = new SelectList(productos, "IdProducto", "Descripcion")
            };
            
            return View(model);
        }

        // POST: Procesa la relación.
        [HttpPost]
        public IActionResult AgregarProducto(AgregarProductoViewModel model)
        {
            // 1. VALIDACIÓN
            if (!ModelState.IsValid)
            {
                // ❌ FALLO CRÍTICO COMÚN EN EXÁMENES:
                // Si la validación falla (ej: Cantidad negativa), volvemos a la vista.
                // PERO... el objeto 'SelectList' se perdió porque HTML no envía listas completas de vuelta.
                // SI NO LO RECARGAMOS AQUÍ, LA VISTA LANZARÁ UNA EXCEPCIÓN (NullReference en el foreach del select).
                
                // Debugging: Imprimir errores en consola para ayudar al desarrollo.
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Console.WriteLine($"Error en el campo '{modelStateKey}': {error.ErrorMessage}");
                    }
                }
        
                // RECARGA DEL DROPDOWN (Obligatorio antes de return View).
                var productos = _productoRepo.GetAll();
                model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");

                return View(model);
            }

            // 🟢 2. PERSISTENCIA RELACIONAL
            // Llamamos al método especial del repositorio que hace el INSERT en la tabla intermedia.
            _repo.AddDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);

            // 3. REDIRECCIÓN
            // Volvemos al detalle del presupuesto para ver el producto recién agregado.
            return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
        }

        // --------------------------------------------------------------------------------
        // MÉTODOS AUXILIARES Y ERRORES
        // --------------------------------------------------------------------------------
        
        public IActionResult AccesoDenegado()
        {
            // Muestra la vista estática "AccesoDenegado.cshtml".
            return View();
        }    
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}