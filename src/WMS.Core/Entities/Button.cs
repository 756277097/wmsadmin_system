namespace WMS.Core.Entities;

/// <summary>
/// 按钮实体
/// </summary>
public class Button
{
    public int Id { get; set; }
    
    /// <summary>
    /// 按钮名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 按钮编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public int MenuId { get; set; }
    
    /// <summary>
    /// 按钮类型：0-查看，1-新增，2-编辑，3-删除，4-其他
    /// </summary>
    public int ButtonType { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
    
    /// <summary>
    /// 所属菜单
    /// </summary>
    public virtual Menu Menu { get; set; } = null!;
    
    /// <summary>
    /// 角色权限关联
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

