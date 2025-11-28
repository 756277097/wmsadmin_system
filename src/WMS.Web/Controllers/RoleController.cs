using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class RoleController : Controller
{
    private readonly IRoleService _roleService;
    private readonly IMenuService _menuService;

    public RoleController(IRoleService roleService, IMenuService menuService)
    {
        _roleService = roleService;
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleService.GetAllAsync();
        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Menus = await _menuService.GetTreeAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleDto dto, string permissionsJson)
    {
        if (await _roleService.ExistsAsync(dto.Code))
        {
            ViewBag.Error = "角色编码已存在";
            ViewBag.Menus = await _menuService.GetTreeAsync();
            return View(dto);
        }

        // 解析权限JSON
        if (!string.IsNullOrEmpty(permissionsJson))
        {
            try
            {
                dto.Permissions = System.Text.Json.JsonSerializer.Deserialize<List<PermissionDto>>(permissionsJson) ?? new List<PermissionDto>();
            }
            catch
            {
                dto.Permissions = new List<PermissionDto>();
            }
        }

        await _roleService.CreateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
            return NotFound();
        ViewBag.Menus = await _menuService.GetTreeAsync();
        return View(role);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RoleDto dto, string permissionsJson)
    {
        // 解析权限JSON
        if (!string.IsNullOrEmpty(permissionsJson))
        {
            try
            {
                dto.Permissions = System.Text.Json.JsonSerializer.Deserialize<List<PermissionDto>>(permissionsJson) ?? new List<PermissionDto>();
            }
            catch
            {
                dto.Permissions = new List<PermissionDto>();
            }
        }

        await _roleService.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _roleService.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var roles = await _roleService.GetAllAsync();
        return Json(roles);
    }
}

