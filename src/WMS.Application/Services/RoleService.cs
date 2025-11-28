using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class RoleService : IRoleService
{
    private readonly WmsDbContext _context;

    public RoleService(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .ToListAsync();

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Description = r.Description,
            IsEnabled = r.IsEnabled,
            Permissions = r.RolePermissions.Select(rp => new PermissionDto
            {
                MenuId = rp.MenuId,
                ButtonId = rp.ButtonId,
                PermissionType = rp.PermissionType
            }).ToList()
        }).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsEnabled = role.IsEnabled,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                MenuId = rp.MenuId,
                ButtonId = rp.ButtonId,
                PermissionType = rp.PermissionType
            }).ToList()
        };
    }

    public async Task<RoleDto> CreateAsync(RoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            IsEnabled = dto.IsEnabled,
            CreateTime = DateTime.Now
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // 添加权限
        foreach (var permission in dto.Permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                MenuId = permission.MenuId,
                ButtonId = permission.ButtonId,
                PermissionType = permission.PermissionType,
                CreateTime = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        dto.Id = role.Id;
        return dto;
    }

    public async Task UpdateAsync(RoleDto dto)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == dto.Id);

        if (role == null)
            throw new Exception("角色不存在");

        role.Name = dto.Name;
        role.Code = dto.Code;
        role.Description = dto.Description;
        role.IsEnabled = dto.IsEnabled;
        role.UpdateTime = DateTime.Now;

        // 更新权限
        _context.RolePermissions.RemoveRange(role.RolePermissions);
        foreach (var permission in dto.Permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                MenuId = permission.MenuId,
                ButtonId = permission.ButtonId,
                PermissionType = permission.PermissionType,
                CreateTime = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string code)
    {
        return await _context.Roles.AnyAsync(r => r.Code == code);
    }
}

