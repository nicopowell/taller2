using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult Error403()
    {
        return View();  // Esto devolverá la vista Views/Error/403.cshtml
    }
}