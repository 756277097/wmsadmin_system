using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class MaterialService : IMaterialService
{
    private readonly WmsDbContext _context;

    public MaterialService(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<MaterialDto>> GetAllAsync()
    {
        var materials = await _context.Materials
            .OrderBy(m => m.Code)
            .ToListAsync();

        return materials.Select(m => new MaterialDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Specification = m.Specification,
            BarcodeType = m.BarcodeType,
            MaterialType = m.MaterialType,
            BaseUnit = m.BaseUnit,
            AuxiliaryUnit = m.AuxiliaryUnit,
            UnitConversion = m.UnitConversion,
            ShelfLife = m.ShelfLife,
            ValidityPeriod = m.ValidityPeriod,
            StorageTempMin = m.StorageTempMin,
            StorageTempMax = m.StorageTempMax,
            StorageHumidityMin = m.StorageHumidityMin,
            StorageHumidityMax = m.StorageHumidityMax,
            StorageRequirements = m.StorageRequirements,
            IsEnabled = m.IsEnabled,
            Remarks = m.Remarks
        }).ToList();
    }

    public async Task<MaterialDto?> GetByIdAsync(int id)
    {
        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == id);

        if (material == null)
            return null;

        return new MaterialDto
        {
            Id = material.Id,
            Code = material.Code,
            Name = material.Name,
            Specification = material.Specification,
            BarcodeType = material.BarcodeType,
            MaterialType = material.MaterialType,
            BaseUnit = material.BaseUnit,
            AuxiliaryUnit = material.AuxiliaryUnit,
            UnitConversion = material.UnitConversion,
            ShelfLife = material.ShelfLife,
            ValidityPeriod = material.ValidityPeriod,
            StorageTempMin = material.StorageTempMin,
            StorageTempMax = material.StorageTempMax,
            StorageHumidityMin = material.StorageHumidityMin,
            StorageHumidityMax = material.StorageHumidityMax,
            StorageRequirements = material.StorageRequirements,
            IsEnabled = material.IsEnabled,
            Remarks = material.Remarks
        };
    }

    public async Task<MaterialDto> CreateAsync(MaterialDto dto)
    {
        var material = new Material
        {
            Code = dto.Code,
            Name = dto.Name,
            Specification = dto.Specification,
            BarcodeType = dto.BarcodeType,
            MaterialType = dto.MaterialType,
            BaseUnit = dto.BaseUnit,
            AuxiliaryUnit = dto.AuxiliaryUnit,
            UnitConversion = dto.UnitConversion,
            ShelfLife = dto.ShelfLife,
            ValidityPeriod = dto.ValidityPeriod,
            StorageTempMin = dto.StorageTempMin,
            StorageTempMax = dto.StorageTempMax,
            StorageHumidityMin = dto.StorageHumidityMin,
            StorageHumidityMax = dto.StorageHumidityMax,
            StorageRequirements = dto.StorageRequirements,
            IsEnabled = dto.IsEnabled,
            Remarks = dto.Remarks,
            CreateTime = DateTime.Now
        };

        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        dto.Id = material.Id;
        return dto;
    }

    public async Task UpdateAsync(MaterialDto dto)
    {
        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == dto.Id);

        if (material == null)
            throw new Exception("物料不存在");

        material.Code = dto.Code;
        material.Name = dto.Name;
        material.Specification = dto.Specification;
        material.BarcodeType = dto.BarcodeType;
        material.MaterialType = dto.MaterialType;
        material.BaseUnit = dto.BaseUnit;
        material.AuxiliaryUnit = dto.AuxiliaryUnit;
        material.UnitConversion = dto.UnitConversion;
        material.ShelfLife = dto.ShelfLife;
        material.ValidityPeriod = dto.ValidityPeriod;
        material.StorageTempMin = dto.StorageTempMin;
        material.StorageTempMax = dto.StorageTempMax;
        material.StorageHumidityMin = dto.StorageHumidityMin;
        material.StorageHumidityMax = dto.StorageHumidityMax;
        material.StorageRequirements = dto.StorageRequirements;
        material.IsEnabled = dto.IsEnabled;
        material.Remarks = dto.Remarks;
        material.UpdateTime = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var material = await _context.Materials.FindAsync(id);
        if (material != null)
        {
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string code)
    {
        return await _context.Materials.AnyAsync(m => m.Code == code);
    }
}

