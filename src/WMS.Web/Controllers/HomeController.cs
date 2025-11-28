using Microsoft.AspNetCore.Mvc;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IAuthService _authService;

    public HomeController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // 注意：权限数据已由 RefreshPermissionFilter 自动刷新，无需在此重复刷新
        return View();
    }
}

