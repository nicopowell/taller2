using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Repositorios;
using MVC.ViewModels; 
using MVC.Interfaces;
namespace MVC.Controllers;


public class ProductosController : Controller
{


    private  IProductoRepository _repo; //= new ProductoRepository(); 
    private  IAuthenticationService _authService;
    public ProductosController( IProductoRepository prodRepo, IAuthenticationService authService)
    {
        //_repo = new ProductoRepository();
        _repo =prodRepo;
            
        _authService=authService;
    }

    public IActionResult Index()
    {
          
       // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        List<Producto> productos = _repo.GetAll(); 
        return View(productos);
    }
    public IActionResult Details(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        Producto producto = _repo.GetById(id);
        if (producto == null) return NotFound();
        return View(producto);
    }


    public IActionResult Create()
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;
        return View();
    }

    /* // POST: Productos/Create
     [HttpPost]

     public IActionResult Create(Producto producto)
     {
         _repo.Add(producto);
         return RedirectToAction(nameof(Index));
     }
 */
 
[HttpPost]
    public IActionResult Create(ProductoViewModel productoVM) 
    {
        // ❗ 1. CHEQUEO DE SEGURIDAD
        if (!ModelState.IsValid)
        {
            // ❌ Si falla: Retorna a la vista con el VM para mostrar los errores
            return View(productoVM); 
        }
        
        // 🟢 2. Mapeo Manual (VM -> Modelo de Dominio)
        var nuevoProducto = new Producto
        {
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio 
        };

        _repo.Add(nuevoProducto); 
        return RedirectToAction(nameof(Index)); 
    }


    // GET: Productos/Edit/5
    public IActionResult Edit(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        Producto producto = _repo.GetById(id);
        ProductoViewModel productovm = new ProductoViewModel(producto);
       
        if (producto == null)
        {
            return NotFound();
        }
        return View(productovm);
    }

    /* // POST: Productos/Edit/
     [HttpPost]

     public IActionResult Edit(int id, Producto producto)
     {
         if (id != producto.IdProducto) return NotFound();
         _repo.Update(producto);
         return RedirectToAction(nameof(Index));
     }
     */

// POST: Productos/Edit/5
    [HttpPost]
    public IActionResult Edit(int id, ProductoViewModel productoVM)
    {
        if (id != productoVM.IdProducto) return NotFound();

        // ❗ 1. CHEQUEO DE SEGURIDAD
        if (!ModelState.IsValid)
        {
            return View(productoVM); 
        }

        // 🟢 2. Mapeo Manual (VM -> Modelo de Dominio)
        var productoAEditar = new Producto
        {
            IdProducto = productoVM.IdProducto,
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio
        };

        _repo.Update(productoAEditar);
        return RedirectToAction(nameof(Index));
    }

    // GET: Productos/Delete/
    public IActionResult Delete(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        Producto producto = _repo.GetById(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: Productos/Delete/
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repo.Delete(id); 
        return RedirectToAction(nameof(Index));
    }

    


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private IActionResult CheckAdminPermissions()
    {
        // 1. No logueado? -> Login (Punto 2.e.iii)
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }
        
        // 2. No es Administrador? -> Error (Punto 2.e.i)
        if (!_authService.HasAccessLevel("Administrador"))
        {
            // Usamos Error403 o redirigimos al Login si no existe vista de error.
            return RedirectToAction(nameof(AccesoDenegado)); 
        }
        return null; // Permiso concedido
    }

    public IActionResult AccesoDenegado()
{
    // El usuario está logueado, pero no tiene el rol suficiente.
    return View();
}   
}
