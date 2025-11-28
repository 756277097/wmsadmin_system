using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(UserDto dto, string password);
    Task UpdateAsync(UserDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string userName);
}

