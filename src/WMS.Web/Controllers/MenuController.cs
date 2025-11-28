using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var menus = await _menuService.GetTreeAsync();
        return View(menus);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? parentId)
    {
        ViewBag.Menus = await _menuService.GetTreeAsync();
        ViewBag.ParentId = parentId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MenuDto dto)
    {
        if (await _menuService.ExistsAsync(dto.Code))
        {
            ViewBag.Error = "菜单编码已存在";
            ViewBag.Menus = await _menuService.GetTreeAsync();
            return View(dto);
        }

        await _menuService.CreateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        if (menu == null)
            return NotFound();
        ViewBag.Menus = await _menuService.GetTreeAsync();
        return View(menu);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MenuDto dto)
    {
        await _menuService.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _menuService.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetTree()
    {
        var menus = await _menuService.GetTreeAsync();
        return Json(menus);
    }
}

