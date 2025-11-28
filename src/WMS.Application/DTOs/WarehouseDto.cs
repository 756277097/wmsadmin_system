namespace WMS.Application.DTOs;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Remarks { get; set; }
    public List<WarehouseZoneDto> Zones { get; set; } = new();
}

public class WarehouseZoneDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public int ZoneType { get; set; } = 0; // 0-原料区，1-成品区，2-暂存区，3-退料区，4-不良品区
    public bool IsEnabled { get; set; } = true;
    public string? Remarks { get; set; }
    public List<StorageLocationDto> Locations { get; set; } = new();
}

public class StorageLocationDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public int LocationType { get; set; } = 0; // 0-固定，1-随机
    public decimal? VolumeLimit { get; set; }
    public decimal? WeightLimit { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Remarks { get; set; }
}

