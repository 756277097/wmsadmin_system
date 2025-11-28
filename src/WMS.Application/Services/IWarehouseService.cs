using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IWarehouseService
{
    // 仓库
    Task<List<WarehouseDto>> GetAllWarehousesAsync();
    Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
    Task<WarehouseDto> CreateWarehouseAsync(WarehouseDto dto);
    Task UpdateWarehouseAsync(WarehouseDto dto);
    Task DeleteWarehouseAsync(int id);
    Task<bool> WarehouseExistsAsync(string code);

    // 库区
    Task<WarehouseZoneDto?> GetZoneByIdAsync(int id);
    Task<WarehouseZoneDto> CreateZoneAsync(WarehouseZoneDto dto);
    Task UpdateZoneAsync(WarehouseZoneDto dto);
    Task DeleteZoneAsync(int id);
    Task<bool> ZoneExistsAsync(string code);

    // 库位
    Task<StorageLocationDto?> GetLocationByIdAsync(int id);
    Task<StorageLocationDto> CreateLocationAsync(StorageLocationDto dto);
    Task UpdateLocationAsync(StorageLocationDto dto);
    Task DeleteLocationAsync(int id);
    Task<bool> LocationExistsAsync(string code);
}

