namespace WMS.Application.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public int BarcodeType { get; set; } = 0; // 0-单码，1-批次码，2-序列号
    public int MaterialType { get; set; } = 0; // 0-原料，1-半成品，2-成品，3-备品备件
    public string BaseUnit { get; set; } = string.Empty;
    public string? AuxiliaryUnit { get; set; }
    public decimal? UnitConversion { get; set; }
    public int? ShelfLife { get; set; }
    public int? ValidityPeriod { get; set; }
    public decimal? StorageTempMin { get; set; }
    public decimal? StorageTempMax { get; set; }
    public decimal? StorageHumidityMin { get; set; }
    public decimal? StorageHumidityMax { get; set; }
    public string? StorageRequirements { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Remarks { get; set; }
}

