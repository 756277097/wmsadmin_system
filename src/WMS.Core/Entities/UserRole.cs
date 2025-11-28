namespace WMS.Core.Entities;

/// <summary>
/// 用户角色关联表
/// </summary>
public class UserRole
{
    public int Id { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 用户
    /// </summary>
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// 角色
    /// </summary>
    public virtual Role Role { get; set; } = null!;
}

