using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IMaterialService
{
    Task<List<MaterialDto>> GetAllAsync();
    Task<MaterialDto?> GetByIdAsync(int id);
    Task<MaterialDto> CreateAsync(MaterialDto dto);
    Task UpdateAsync(MaterialDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string code);
}

