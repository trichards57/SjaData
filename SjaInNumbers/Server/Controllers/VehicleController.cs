using Microsoft.AspNetCore.Mvc;

namespace SjaInNumbers.Server.Controllers;
public class VehicleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
