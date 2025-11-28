using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "用户名和密码不能为空";
            return View();
        }

        var user = await _authService.LoginAsync(userName, password);
        if (user == null)
        {
            ViewBag.Error = "用户名或密码错误";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.UserName);
        HttpContext.Session.SetString("RealName", user.RealName ?? user.UserName);

        // 获取用户菜单和权限
        var menus = await _authService.GetUserMenusAsync(user.Id);
        var permissions = await _authService.GetUserPermissionsAsync(user.Id);
        
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        HttpContext.Session.SetString("UserMenus", System.Text.Json.JsonSerializer.Serialize(menus, jsonOptions));
        HttpContext.Session.SetString("UserPermissions", System.Text.Json.JsonSerializer.Serialize(permissions, jsonOptions));

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMenus()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Json(new List<object>());
        }

        var menus = await _authService.GetUserMenusAsync(userId.Value);
        return Json(menus);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPermissions()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Json(new List<string>());
        }

        var permissions = await _authService.GetUserPermissionsAsync(userId.Value);
        return Json(permissions);
    }
}

