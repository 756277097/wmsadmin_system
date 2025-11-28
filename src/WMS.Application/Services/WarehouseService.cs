using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly WmsDbContext _context;

    public WarehouseService(WmsDbContext context)
    {
        _context = context;
    }

    #region 仓库

    public async Task<List<WarehouseDto>> GetAllWarehousesAsync()
    {
        var warehouses = await _context.Warehouses
            .Include(w => w.Zones)
                .ThenInclude(z => z.Locations)
            .OrderBy(w => w.Code)
            .ToListAsync();

        return warehouses.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Code = w.Code,
            Name = w.Name,
            Address = w.Address,
            ContactPerson = w.ContactPerson,
            ContactPhone = w.ContactPhone,
            IsEnabled = w.IsEnabled,
            Remarks = w.Remarks,
            Zones = w.Zones.Select(z => new WarehouseZoneDto
            {
                Id = z.Id,
                Code = z.Code,
                Name = z.Name,
                WarehouseId = z.WarehouseId,
                ZoneType = z.ZoneType,
                IsEnabled = z.IsEnabled,
                Remarks = z.Remarks,
                Locations = z.Locations.Select(l => new StorageLocationDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Name = l.Name,
                    ZoneId = l.ZoneId,
                    LocationType = l.LocationType,
                    VolumeLimit = l.VolumeLimit,
                    WeightLimit = l.WeightLimit,
                    IsEnabled = l.IsEnabled,
                    Remarks = l.Remarks
                }).OrderBy(l => l.Code).ToList()
            }).OrderBy(z => z.Code).ToList()
        }).ToList();
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(w => w.Zones)
                .ThenInclude(z => z.Locations)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse == null)
            return null;

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Address = warehouse.Address,
            ContactPerson = warehouse.ContactPerson,
            ContactPhone = warehouse.ContactPhone,
            IsEnabled = warehouse.IsEnabled,
            Remarks = warehouse.Remarks,
            Zones = warehouse.Zones.Select(z => new WarehouseZoneDto
            {
                Id = z.Id,
                Code = z.Code,
                Name = z.Name,
                WarehouseId = z.WarehouseId,
                ZoneType = z.ZoneType,
                IsEnabled = z.IsEnabled,
                Remarks = z.Remarks,
                Locations = z.Locations.Select(l => new StorageLocationDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Name = l.Name,
                    ZoneId = l.ZoneId,
                    LocationType = l.LocationType,
                    VolumeLimit = l.VolumeLimit,
                    WeightLimit = l.WeightLimit,
                    IsEnabled = l.IsEnabled,
                    Remarks = l.Remarks
                }).OrderBy(l => l.Code).ToList()
            }).OrderBy(z => z.Code).ToList()
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(WarehouseDto dto)
    {
        var warehouse = new Warehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Address = dto.Address,
            ContactPerson = dto.ContactPerson,
            ContactPhone = dto.ContactPhone,
            IsEnabled = dto.IsEnabled,
            Remarks = dto.Remarks,
            CreateTime = DateTime.Now
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        dto.Id = warehouse.Id;
        return dto;
    }

    public async Task UpdateWarehouseAsync(WarehouseDto dto)
    {
        var warehouse = await _context.Warehouses.FindAsync(dto.Id);
        if (warehouse == null)
            throw new Exception("仓库不存在");

        warehouse.Code = dto.Code;
        warehouse.Name = dto.Name;
        warehouse.Address = dto.Address;
        warehouse.ContactPerson = dto.ContactPerson;
        warehouse.ContactPhone = dto.ContactPhone;
        warehouse.IsEnabled = dto.IsEnabled;
        warehouse.Remarks = dto.Remarks;
        warehouse.UpdateTime = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteWarehouseAsync(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> WarehouseExistsAsync(string code)
    {
        return await _context.Warehouses.AnyAsync(w => w.Code == code);
    }

    #endregion

    #region 库区

    public async Task<WarehouseZoneDto?> GetZoneByIdAsync(int id)
    {
        var zone = await _context.WarehouseZones
            .Include(z => z.Locations)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zone == null)
            return null;

        return new WarehouseZoneDto
        {
            Id = zone.Id,
            Code = zone.Code,
            Name = zone.Name,
            WarehouseId = zone.WarehouseId,
            ZoneType = zone.ZoneType,
            IsEnabled = zone.IsEnabled,
            Remarks = zone.Remarks,
            Locations = zone.Locations.Select(l => new StorageLocationDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                ZoneId = l.ZoneId,
                LocationType = l.LocationType,
                VolumeLimit = l.VolumeLimit,
                WeightLimit = l.WeightLimit,
                IsEnabled = l.IsEnabled,
                Remarks = l.Remarks
            }).OrderBy(l => l.Code).ToList()
        };
    }

    public async Task<WarehouseZoneDto> CreateZoneAsync(WarehouseZoneDto dto)
    {
        var zone = new WarehouseZone
        {
            Code = dto.Code,
            Name = dto.Name,
            WarehouseId = dto.WarehouseId,
            ZoneType = dto.ZoneType,
            IsEnabled = dto.IsEnabled,
            Remarks = dto.Remarks,
            CreateTime = DateTime.Now
        };

        _context.WarehouseZones.Add(zone);
        await _context.SaveChangesAsync();

        dto.Id = zone.Id;
        return dto;
    }

    public async Task UpdateZoneAsync(WarehouseZoneDto dto)
    {
        var zone = await _context.WarehouseZones.FindAsync(dto.Id);
        if (zone == null)
            throw new Exception("库区不存在");

        zone.Code = dto.Code;
        zone.Name = dto.Name;
        zone.WarehouseId = dto.WarehouseId;
        zone.ZoneType = dto.ZoneType;
        zone.IsEnabled = dto.IsEnabled;
        zone.Remarks = dto.Remarks;
        zone.UpdateTime = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteZoneAsync(int id)
    {
        var zone = await _context.WarehouseZones.FindAsync(id);
        if (zone != null)
        {
            _context.WarehouseZones.Remove(zone);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ZoneExistsAsync(string code)
    {
        return await _context.WarehouseZones.AnyAsync(z => z.Code == code);
    }

    #endregion

    #region 库位

    public async Task<StorageLocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _context.StorageLocations.FindAsync(id);
        if (location == null)
            return null;

        return new StorageLocationDto
        {
            Id = location.Id,
            Code = location.Code,
            Name = location.Name,
            ZoneId = location.ZoneId,
            LocationType = location.LocationType,
            VolumeLimit = location.VolumeLimit,
            WeightLimit = location.WeightLimit,
            IsEnabled = location.IsEnabled,
            Remarks = location.Remarks
        };
    }

    public async Task<StorageLocationDto> CreateLocationAsync(StorageLocationDto dto)
    {
        var location = new StorageLocation
        {
            Code = dto.Code,
            Name = dto.Name,
            ZoneId = dto.ZoneId,
            LocationType = dto.LocationType,
            VolumeLimit = dto.VolumeLimit,
            WeightLimit = dto.WeightLimit,
            IsEnabled = dto.IsEnabled,
            Remarks = dto.Remarks,
            CreateTime = DateTime.Now
        };

        _context.StorageLocations.Add(location);
        await _context.SaveChangesAsync();

        dto.Id = location.Id;
        return dto;
    }

    public async Task UpdateLocationAsync(StorageLocationDto dto)
    {
        var location = await _context.StorageLocations.FindAsync(dto.Id);
        if (location == null)
            throw new Exception("库位不存在");

        location.Code = dto.Code;
        location.Name = dto.Name;
        location.ZoneId = dto.ZoneId;
        location.LocationType = dto.LocationType;
        location.VolumeLimit = dto.VolumeLimit;
        location.WeightLimit = dto.WeightLimit;
        location.IsEnabled = dto.IsEnabled;
        location.Remarks = dto.Remarks;
        location.UpdateTime = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteLocationAsync(int id)
    {
        var location = await _context.StorageLocations.FindAsync(id);
        if (location != null)
        {
            _context.StorageLocations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> LocationExistsAsync(string code)
    {
        return await _context.StorageLocations.AnyAsync(l => l.Code == code);
    }

    #endregion
}

