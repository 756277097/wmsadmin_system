using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class MaterialController : Controller
{
    private readonly IMaterialService _materialService;

    public MaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var materials = await _materialService.GetAllAsync();
        return View(materials);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MaterialDto dto)
    {
        if (await _materialService.ExistsAsync(dto.Code))
        {
            ViewBag.Error = "物料编码已存在";
            return View(dto);
        }

        await _materialService.CreateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var material = await _materialService.GetByIdAsync(id);
        if (material == null)
            return NotFound();
        return View(material);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MaterialDto dto)
    {
        // 检查编码是否被其他物料使用
        var existing = await _materialService.GetByIdAsync(dto.Id);
        if (existing == null)
            return NotFound();

        if (existing.Code != dto.Code && await _materialService.ExistsAsync(dto.Code))
        {
            ViewBag.Error = "物料编码已存在";
            return View(dto);
        }

        await _materialService.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _materialService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}

