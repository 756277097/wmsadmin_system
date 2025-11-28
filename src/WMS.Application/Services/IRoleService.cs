using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(RoleDto dto);
    Task UpdateAsync(RoleDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string code);
}

