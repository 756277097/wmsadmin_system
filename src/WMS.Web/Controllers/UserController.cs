using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserDto dto, string password)
    {
        if (await _userService.ExistsAsync(dto.UserName))
        {
            ViewBag.Error = "用户名已存在";
            return View(dto);
        }

        await _userService.CreateAsync(dto, password);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserDto dto, string roleIds)
    {
        // 解析角色ID字符串
        if (!string.IsNullOrEmpty(roleIds))
        {
            var ids = roleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var result) ? result : 0)
                .Where(id => id > 0)
                .ToList();
            dto.RoleIds = ids;
        }

        await _userService.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var users = await _userService.GetAllAsync();
        return Json(users);
    }
}

public class RoleApiController : Controller
{
    private readonly IRoleService _roleService;

    public RoleApiController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Json(roles);
    }
}

