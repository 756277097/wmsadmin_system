namespace WMS.Core.Entities;

/// <summary>
/// 库区实体
/// </summary>
public class WarehouseZone
{
    public int Id { get; set; }
    
    /// <summary>
    /// 库区编码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 库区名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 仓库ID
    /// </summary>
    public int WarehouseId { get; set; }
    
    /// <summary>
    /// 库区类型：0-原料区，1-成品区，2-暂存区，3-退料区，4-不良品区
    /// </summary>
    public int ZoneType { get; set; } = 0;
    
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
    /// 所属仓库
    /// </summary>
    public virtual Warehouse? Warehouse { get; set; }
    
    /// <summary>
    /// 库位集合
    /// </summary>
    public virtual ICollection<StorageLocation> Locations { get; set; } = new List<StorageLocation>();
}

