using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IMenuService
{
    Task<List<MenuDto>> GetAllAsync();
    Task<List<MenuDto>> GetTreeAsync();
    Task<MenuDto?> GetByIdAsync(int id);
    Task<MenuDto> CreateAsync(MenuDto dto);
    Task UpdateAsync(MenuDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string code);
}

