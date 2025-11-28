using WMS.Application.DTOs;

namespace WMS.Application.Services;

public interface IAuthService
{
    Task<UserDto?> LoginAsync(string userName, string password);
    Task<List<MenuDto>> GetUserMenusAsync(int userId);
    Task<List<string>> GetUserPermissionsAsync(int userId);
}

