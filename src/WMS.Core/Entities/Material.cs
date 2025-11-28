namespace WMS.Core.Entities;

/// <summary>
/// 物料实体
/// </summary>
public class Material
{
    public int Id { get; set; }
    
    /// <summary>
    /// 物料编码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 物料名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Specification { get; set; }
    
    /// <summary>
    /// 物料条码类型：0-单码，1-批次码，2-序列号
    /// </summary>
    public int BarcodeType { get; set; } = 0;
    
    /// <summary>
    /// 物料类型：0-原料，1-半成品，2-成品，3-备品备件
    /// </summary>
    public int MaterialType { get; set; } = 0;
    
    /// <summary>
    /// 基本单位
    /// </summary>
    public string BaseUnit { get; set; } = string.Empty;
    
    /// <summary>
    /// 辅助单位
    /// </summary>
    public string? AuxiliaryUnit { get; set; }
    
    /// <summary>
    /// 单位换算关系（辅助单位:基本单位，例如：1箱=12件）
    /// </summary>
    public decimal? UnitConversion { get; set; }
    
    /// <summary>
    /// 保质期（天数）
    /// </summary>
    public int? ShelfLife { get; set; }
    
    /// <summary>
    /// 有效期（天数）
    /// </summary>
    public int? ValidityPeriod { get; set; }
    
    /// <summary>
    /// 保管要求-温度（最低）
    /// </summary>
    public decimal? StorageTempMin { get; set; }
    
    /// <summary>
    /// 保管要求-温度（最高）
    /// </summary>
    public decimal? StorageTempMax { get; set; }
    
    /// <summary>
    /// 保管要求-湿度（最低）
    /// </summary>
    public decimal? StorageHumidityMin { get; set; }
    
    /// <summary>
    /// 保管要求-湿度（最高）
    /// </summary>
    public decimal? StorageHumidityMax { get; set; }
    
    /// <summary>
    /// 其他保管要求
    /// </summary>
    public string? StorageRequirements { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}

