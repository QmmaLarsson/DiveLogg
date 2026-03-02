using Microsoft.AspNetCore.Mvc;

namespace DiveLogg.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
}