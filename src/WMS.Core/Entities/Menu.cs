namespace WMS.Core.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu
{
    public int Id { get; set; }
    
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 父菜单ID（null表示顶级菜单）
    /// </summary>
    public int? ParentId { get; set; }
    
    /// <summary>
    /// 菜单类型：0-内部页面，1-外部链接
    /// </summary>
    public int MenuType { get; set; }
    
    /// <summary>
    /// 路由地址（内部页面）或链接地址（外部链接）
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }
    
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
    /// 子菜单
    /// </summary>
    public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
    
    /// <summary>
    /// 父菜单
    /// </summary>
    public virtual Menu? Parent { get; set; }
    
    /// <summary>
    /// 菜单下的按钮
    /// </summary>
    public virtual ICollection<Button> Buttons { get; set; } = new List<Button>();
    
    /// <summary>
    /// 角色权限关联
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

