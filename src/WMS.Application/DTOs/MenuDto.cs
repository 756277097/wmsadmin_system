namespace WMS.Application.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int MenuType { get; set; }
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; }
    public List<MenuDto> Children { get; set; } = new();
    public List<ButtonDto> Buttons { get; set; } = new();
}

public class ButtonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int MenuId { get; set; }
    public int ButtonType { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; }
}

