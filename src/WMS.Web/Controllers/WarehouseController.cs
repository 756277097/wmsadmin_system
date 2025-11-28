using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Application.Services;

namespace WMS.Web.Controllers;

public class WarehouseController : Controller
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        return View(warehouses);
    }

    // 仓库操作
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(WarehouseDto dto)
    {
        if (await _warehouseService.WarehouseExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "仓库编码已存在" });
        }

        await _warehouseService.CreateWarehouseAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateWarehouse(WarehouseDto dto)
    {
        var existing = await _warehouseService.GetWarehouseByIdAsync(dto.Id);
        if (existing == null)
            return Json(new { success = false, message = "仓库不存在" });

        if (existing.Code != dto.Code && await _warehouseService.WarehouseExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "仓库编码已存在" });
        }

        await _warehouseService.UpdateWarehouseAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWarehouse([FromBody] int id)
    {
        await _warehouseService.DeleteWarehouseAsync(id);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetWarehouse(int id)
    {
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
        if (warehouse == null)
            return NotFound();
        return Json(warehouse);
    }

    // 库区操作
    [HttpPost]
    public async Task<IActionResult> CreateZone(WarehouseZoneDto dto)
    {
        if (await _warehouseService.ZoneExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "库区编码已存在" });
        }

        await _warehouseService.CreateZoneAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateZone(WarehouseZoneDto dto)
    {
        var existing = await _warehouseService.GetZoneByIdAsync(dto.Id);
        if (existing == null)
            return Json(new { success = false, message = "库区不存在" });

        if (existing.Code != dto.Code && await _warehouseService.ZoneExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "库区编码已存在" });
        }

        await _warehouseService.UpdateZoneAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteZone([FromBody] int id)
    {
        await _warehouseService.DeleteZoneAsync(id);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetZone(int id)
    {
        var zone = await _warehouseService.GetZoneByIdAsync(id);
        if (zone == null)
            return NotFound();
        return Json(zone);
    }

    // 库位操作
    [HttpPost]
    public async Task<IActionResult> CreateLocation(StorageLocationDto dto)
    {
        if (await _warehouseService.LocationExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "库位编码已存在" });
        }

        await _warehouseService.CreateLocationAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateLocation(StorageLocationDto dto)
    {
        var existing = await _warehouseService.GetLocationByIdAsync(dto.Id);
        if (existing == null)
            return Json(new { success = false, message = "库位不存在" });

        if (existing.Code != dto.Code && await _warehouseService.LocationExistsAsync(dto.Code))
        {
            return Json(new { success = false, message = "库位编码已存在" });
        }

        await _warehouseService.UpdateLocationAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLocation([FromBody] int id)
    {
        await _warehouseService.DeleteLocationAsync(id);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetLocation(int id)
    {
        var location = await _warehouseService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();
        return Json(location);
    }
}

