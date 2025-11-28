namespace WMS.Core.Entities;

/// <summary>
/// 角色权限关联表（菜单和按钮）
/// </summary>
public class RolePermission
{
    public int Id { get; set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// 菜单ID（如果为按钮权限，则同时关联菜单）
    /// </summary>
    public int? MenuId { get; set; }
    
    /// <summary>
    /// 按钮ID（如果为菜单权限，则为null）
    /// </summary>
    public int? ButtonId { get; set; }
    
    /// <summary>
    /// 权限类型：0-菜单，1-按钮
    /// </summary>
    public int PermissionType { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 角色
    /// </summary>
    public virtual Role Role { get; set; } = null!;
    
    /// <summary>
    /// 菜单
    /// </summary>
    public virtual Menu? Menu { get; set; }
    
    /// <summary>
    /// 按钮
    /// </summary>
    public virtual Button? Button { get; set; }
}

