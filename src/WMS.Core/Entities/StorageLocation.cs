namespace WMS.Core.Entities;

/// <summary>
/// 库位实体
/// </summary>
public class StorageLocation
{
    public int Id { get; set; }
    
    /// <summary>
    /// 库位编码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 库位名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 库区ID
    /// </summary>
    public int ZoneId { get; set; }
    
    /// <summary>
    /// 库位属性：0-固定，1-随机
    /// </summary>
    public int LocationType { get; set; } = 0;
    
    /// <summary>
    /// 体积限制（立方米）
    /// </summary>
    public decimal? VolumeLimit { get; set; }
    
    /// <summary>
    /// 重量限制（千克）
    /// </summary>
    public decimal? WeightLimit { get; set; }
    
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
    
    /// <summary>
    /// 所属库区
    /// </summary>
    public virtual WarehouseZone? Zone { get; set; }
}

