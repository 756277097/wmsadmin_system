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

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // 刷新用户菜单和权限（确保数据是最新的）
        try
        {
            var menus = await _authService.GetUserMenusAsync(userId.Value);
            var permissions = await _authService.GetUserPermissionsAsync(userId.Value);
            
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            HttpContext.Session.SetString("UserMenus", System.Text.Json.JsonSerializer.Serialize(menus, jsonOptions));
            HttpContext.Session.SetString("UserPermissions", System.Text.Json.JsonSerializer.Serialize(permissions, jsonOptions));
        }
        catch (Exception ex)
        {
            // 如果刷新失败，记录错误但不阻止页面加载
            Console.WriteLine($"刷新菜单数据失败: {ex.Message}");
        }

        return View();
    }
}

