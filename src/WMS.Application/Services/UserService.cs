using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class UserService : IUserService
{
    private readonly WmsDbContext _context;

    public UserService(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            RealName = u.RealName,
            Email = u.Email,
            Phone = u.Phone,
            IsEnabled = u.IsEnabled,
            RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
        }).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
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

    public async Task<UserDto> CreateAsync(UserDto dto, string password)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Password = HashPassword(password),
            RealName = dto.RealName,
            Email = dto.Email,
            Phone = dto.Phone,
            IsEnabled = dto.IsEnabled,
            CreateTime = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 添加角色关联
        foreach (var roleId in dto.RoleIds)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                CreateTime = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        dto.Id = user.Id;
        return dto;
    }

    public async Task UpdateAsync(UserDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == dto.Id);

        if (user == null)
            throw new Exception("用户不存在");

        user.RealName = dto.RealName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.IsEnabled = dto.IsEnabled;
        user.UpdateTime = DateTime.Now;

        // 更新角色关联
        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        var toAdd = dto.RoleIds.Except(existingRoleIds).ToList();
        var toRemove = existingRoleIds.Except(dto.RoleIds).ToList();

        foreach (var roleId in toAdd)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                CreateTime = DateTime.Now
            });
        }

        foreach (var roleId in toRemove)
        {
            var userRole = user.UserRoles.First(ur => ur.RoleId == roleId);
            _context.UserRoles.Remove(userRole);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

