namespace WMS.Core.Entities;

/// <summary>
/// 仓库实体
/// </summary>
public class Warehouse
{
    public int Id { get; set; }
    
    /// <summary>
    /// 仓库编码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 仓库名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 仓库地址
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// 联系人
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }
    
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
    /// 库区集合
    /// </summary>
    public virtual ICollection<WarehouseZone> Zones { get; set; } = new List<WarehouseZone>();
}

