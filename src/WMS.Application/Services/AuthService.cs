using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly WmsDbContext _context;

    public AuthService(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> LoginAsync(string userName, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == userName && u.IsEnabled);

        if (user == null)
            return null;

        var hashedPassword = HashPassword(password);
        if (user.Password != hashedPassword)
            return null;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            RealName = user.RealName,
            Email = user.Email,
            Phone = user.Phone,
            IsEnabled = user.IsEnabled,
            RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList()
        };
    }

    public async Task<List<MenuDto>> GetUserMenusAsync(int userId)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
        {
            return new List<MenuDto>();
        }

        // 获取用户有权限的菜单ID
        var menuIds = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.PermissionType == 0 && rp.MenuId != null)
            .Select(rp => rp.MenuId!.Value)
            .Distinct()
            .ToListAsync();

        if (!menuIds.Any())
        {
            return new List<MenuDto>();
        }

        // 获取所有有权限的菜单（包括子菜单）
        var allMenus = await _context.Menus
            .Where(m => m.IsEnabled)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        // 递归获取所有父菜单（确保菜单树完整）
        var finalMenuIds = new HashSet<int>(menuIds);
        foreach (var menuId in menuIds)
        {
            AddParentMenus(allMenus, menuId, finalMenuIds);
        }

        // 只获取最终需要的菜单
        var menus = allMenus
            .Where(m => finalMenuIds.Contains(m.Id))
            .OrderBy(m => m.Sort)
            .ToList();

        return BuildMenuTree(menus, null);
    }

    // 递归添加父菜单
    private void AddParentMenus(List<Menu> allMenus, int menuId, HashSet<int> menuIds)
    {
        var menu = allMenus.FirstOrDefault(m => m.Id == menuId);
        if (menu == null || menu.ParentId == null)
            return;

        if (!menuIds.Contains(menu.ParentId.Value))
        {
            menuIds.Add(menu.ParentId.Value);
            AddParentMenus(allMenus, menu.ParentId.Value, menuIds);
        }
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var menuCodes = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.PermissionType == 0 && rp.MenuId != null)
            .Select(rp => rp.Menu!.Code)
            .Distinct()
            .ToListAsync();

        var buttonCodes = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.PermissionType == 1 && rp.ButtonId != null)
            .Select(rp => rp.Button!.Code)
            .Distinct()
            .ToListAsync();

        return menuCodes.Concat(buttonCodes).ToList();
    }

    private List<MenuDto> BuildMenuTree(List<Menu> menus, int? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                ParentId = m.ParentId,
                MenuType = m.MenuType,
                Path = m.Path,
                Icon = m.Icon,
                Sort = m.Sort,
                IsEnabled = m.IsEnabled,
                Children = BuildMenuTree(menus, m.Id)
            })
            .OrderBy(m => m.Sort)
            .ToList();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

