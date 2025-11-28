using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC.Models; // Asumimos que los Modelos están aquí
using MVC.ViewModels; // ❗ IMPORTANTE: Los nuevos ViewModels
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList
using MVC.Interfaces;

namespace MVC.Controllers;


    public class PresupuestosController : Controller
    {
        private  IPresupuestoRepository _repo; //= new PresupuestoRepository();
        // Se necesita el repositorio de Productos para llenar los Dropdowns
        private  IProductoRepository _productoRepo; //= new ProductoRepository(); 
        private  IAuthenticationService _authService;
    public PresupuestosController(IPresupuestoRepository repo, IProductoRepository prodRepo, IAuthenticationService authService)
        {
            _repo =repo;
            _productoRepo=prodRepo;
            _authService=authService;
        }

    // ------------------------------------------------------------------
    // 1. LISTAR (INDEX)
    // ------------------------------------------------------------------
    public IActionResult Index()
        {
            // Comprobación manual de autenticación
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Comprobación manual de nivel de acceso
        if (_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente") )
        {
            //si es admin o cliente entra
        var presupuestos = _repo.GetAll();
        return View(presupuestos);
        }
        else
        {
            return RedirectToAction("Index", "Login");
        }

        
        }

        // ------------------------------------------------------------------
        // 2. DETALLE (DETAILS) - Necesario para la lógica relacional
        // ------------------------------------------------------------------
        public IActionResult Details(int id)
        {
            var presupuesto = _repo.GetById(id);
            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto); // La vista mostrará los detalles y el listado de productos
        }

        // ------------------------------------------------------------------
        // 3. CREAR (CREATE)
        // ------------------------------------------------------------------
        // GET: Presupuestos/Create
        public IActionResult Create()
        {
         
           if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Comprobación manual de nivel de acceso
        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }
           // Se retorna un VM vacío para el formulario
            return View(new PresupuestoViewModel());
        }

        // ❗ POST: Presupuestos/Create (CON VALIDACIÓN)
        [HttpPost]
        public IActionResult Create(PresupuestoViewModel presupuestoVM) 
        {
            // ❗ 1. VALIDACIÓN DE REGLA DE NEGOCIO ESPECÍFICA (Fecha no Futura)
            if (presupuestoVM.FechaCreacion > DateTime.Today)
            {
                // Se añade un error al ModelState
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser una fecha futura.");
            }
            
            // ❗ 2. CHEQUEO DE SEGURIDAD (incluye el error de Fecha si se añadió)
            if (!ModelState.IsValid)
            {
                // ❌ Si falla: Retorna a la misma vista con el VM para mostrar los errores
                return View(presupuestoVM); 
            }
            
            // 🟢 3. SI ES VÁLIDO: Mapeo Manual (VM -> Modelo de Dominio)
            var nuevoPresupuesto = new Presupuesto
            {
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = presupuestoVM.FechaCreacion
            };

            // 4. Llamada al Repositorio
            _repo.Add(nuevoPresupuesto); 
            return RedirectToAction(nameof(Index)); 
        }

        // ------------------------------------------------------------------
        // 4. EDITAR (EDIT)
        // ------------------------------------------------------------------
        // GET: Presupuestos/Edit/5
        public IActionResult Edit(int id)
        {
               if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Comprobación manual de nivel de acceso
        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }
        
        var presupuesto = _repo.GetById(id);
        if (presupuesto == null) return NotFound();

        // Mapeo inverso (Modelo de Dominio -> VM) para la vista GET
        var presupuestoVM = new PresupuestoViewModel(presupuesto);

            return View(presupuestoVM);
        }

        // ❗ POST: Presupuestos/Edit/5 (CON VALIDACIÓN)
        [HttpPost]
        public IActionResult Edit(int id, PresupuestoViewModel presupuestoVM)
        {
            if (id != presupuestoVM.IdPresupuesto) return NotFound();

            // ❗ 1. VALIDACIÓN DE REGLA DE NEGOCIO Específica
            if (presupuestoVM.FechaCreacion > DateTime.Today)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser una fecha futura.");
            }

            // ❗ 2. CHEQUEO DE SEGURIDAD
            if (!ModelState.IsValid)
            {
                // ❌ Si falla: Retorna a la vista con el VM
                return View(presupuestoVM); 
            }

            // 🟢 3. Mapeo Manual (VM -> Modelo de Dominio)
            var presupuestoAEditar = new Presupuesto
            {
                IdPresupuesto = presupuestoVM.IdPresupuesto,
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = presupuestoVM.FechaCreacion
            };

            // 4. Llamada al Repositorio
            _repo.Update(presupuestoAEditar);
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------------
        // 5. ELIMINAR (DELETE)
        // ------------------------------------------------------------------
        // GET: Presupuestos/Delete/5
        public IActionResult Delete(int id)
        {
               if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Comprobación manual de nivel de acceso
        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }

            var presupuesto = _repo.GetById(id);
            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }

        // POST: Presupuestos/Delete/5 (POST de confirmación)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // ==================================================================
        // 6. LÓGICA RELACIONAL N:M (AGREGAR PRODUCTOS)
        // ==================================================================

        // 🔗 GET: Presupuestos/AgregarProducto/5
        public IActionResult AgregarProducto(int id)
        {
               if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Comprobación manual de nivel de acceso
        if (!_authService.HasAccessLevel("Administrador"))
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }
            // 1. Obtener los productos disponibles
            List<Producto> productos = _productoRepo.GetAll();
            
            // 2. Crear el ViewModel
            AgregarProductoViewModel model = new AgregarProductoViewModel
            {
                IdPresupuesto = id, // Pasamos el ID del presupuesto actual
                // 3. Crear el SelectList
                ListaProductos = new SelectList(productos, "IdProducto", "Descripcion")
            };
            
            return View(model);
        }

    // ❗ 🔗 POST: Presupuestos/AgregarProducto (CON VALIDACIÓN DE CANTIDAD)
    [HttpPost]
    public IActionResult AgregarProducto(AgregarProductoViewModel model)
    {
        // 1. Chequeo de Seguridad para la Cantidad
        if (!ModelState.IsValid)
        {
            // ❌ LÓGICA CRÍTICA DE RECARGA: Si la validación falla (ej. Cantidad < 1),
            // Muestra todos los errores en la Consola/Debug Output de Visual Studio
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    // Imprime el nombre del campo y el error de validación exacto.
                    Console.WriteLine($"Error en el campo '{modelStateKey}': {error.ErrorMessage}");
                }
            }
    
            // DEBEMOS recargar el SelectList antes de devolver la vista.
            var productos = _productoRepo.GetAll();
            model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");

            // Devolvemos el modelo con los errores y el dropdown recargado
            return View(model);
        }

        // 🟢 2. Si es VÁLIDO: Llamamos al repositorio para guardar la relación
        _repo.AddDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);

        // 3. Redirigimos al detalle del presupuesto
        return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
    }

    // ❗ Nueva Acción: Simplemente devuelve una vista estática con el mensaje.
public IActionResult AccesoDenegado()
{
    // El usuario está logueado, pero no tiene el rol suficiente.
    return View();
}    
        
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    

    }
