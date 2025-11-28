namespace WMS.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsEnabled { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

