using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Application.Services;

public class MenuService : IMenuService
{
    private readonly WmsDbContext _context;

    public MenuService(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDto>> GetAllAsync()
    {
        var menus = await _context.Menus
            .Include(m => m.Buttons)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        return menus.Select(m => new MenuDto
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
            Buttons = m.Buttons.Select(b => new ButtonDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                MenuId = b.MenuId,
                ButtonType = b.ButtonType,
                Sort = b.Sort,
                IsEnabled = b.IsEnabled
            }).ToList()
        }).ToList();
    }

    public async Task<List<MenuDto>> GetTreeAsync()
    {
        var menus = await _context.Menus
            .Include(m => m.Buttons)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        return BuildMenuTree(menus, null);
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus
            .Include(m => m.Buttons)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
            return null;

        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Code = menu.Code,
            ParentId = menu.ParentId,
            MenuType = menu.MenuType,
            Path = menu.Path,
            Icon = menu.Icon,
            Sort = menu.Sort,
            IsEnabled = menu.IsEnabled,
            Buttons = menu.Buttons.Select(b => new ButtonDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                MenuId = b.MenuId,
                ButtonType = b.ButtonType,
                Sort = b.Sort,
                IsEnabled = b.IsEnabled
            }).ToList()
        };
    }

    public async Task<MenuDto> CreateAsync(MenuDto dto)
    {
        var menu = new Menu
        {
            Name = dto.Name,
            Code = dto.Code,
            ParentId = dto.ParentId,
            MenuType = dto.MenuType,
            Path = dto.Path,
            Icon = dto.Icon,
            Sort = dto.Sort,
            IsEnabled = dto.IsEnabled,
            CreateTime = DateTime.Now
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        // 添加按钮
        foreach (var buttonDto in dto.Buttons)
        {
            _context.Buttons.Add(new Button
            {
                Name = buttonDto.Name,
                Code = buttonDto.Code,
                MenuId = menu.Id,
                ButtonType = buttonDto.ButtonType,
                Sort = buttonDto.Sort,
                IsEnabled = buttonDto.IsEnabled,
                CreateTime = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        dto.Id = menu.Id;
        return dto;
    }

    public async Task UpdateAsync(MenuDto dto)
    {
        var menu = await _context.Menus
            .Include(m => m.Buttons)
            .FirstOrDefaultAsync(m => m.Id == dto.Id);

        if (menu == null)
            throw new Exception("菜单不存在");

        menu.Name = dto.Name;
        menu.Code = dto.Code;
        menu.ParentId = dto.ParentId;
        menu.MenuType = dto.MenuType;
        menu.Path = dto.Path;
        menu.Icon = dto.Icon;
        menu.Sort = dto.Sort;
        menu.IsEnabled = dto.IsEnabled;
        menu.UpdateTime = DateTime.Now;

        // 更新按钮
        var existingButtonIds = menu.Buttons.Select(b => b.Id).ToList();
        var newButtonIds = dto.Buttons.Select(b => b.Id).Where(id => id > 0).ToList();
        var toRemove = existingButtonIds.Except(newButtonIds).ToList();

        foreach (var buttonDto in dto.Buttons)
        {
            if (buttonDto.Id > 0)
            {
                var button = menu.Buttons.FirstOrDefault(b => b.Id == buttonDto.Id);
                if (button != null)
                {
                    button.Name = buttonDto.Name;
                    button.Code = buttonDto.Code;
                    button.ButtonType = buttonDto.ButtonType;
                    button.Sort = buttonDto.Sort;
                    button.IsEnabled = buttonDto.IsEnabled;
                    button.UpdateTime = DateTime.Now;
                }
            }
            else
            {
                _context.Buttons.Add(new Button
                {
                    Name = buttonDto.Name,
                    Code = buttonDto.Code,
                    MenuId = menu.Id,
                    ButtonType = buttonDto.ButtonType,
                    Sort = buttonDto.Sort,
                    IsEnabled = buttonDto.IsEnabled,
                    CreateTime = DateTime.Now
                });
            }
        }

        foreach (var buttonId in toRemove)
        {
            var button = menu.Buttons.First(b => b.Id == buttonId);
            _context.Buttons.Remove(button);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string code)
    {
        return await _context.Menus.AnyAsync(m => m.Code == code);
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
                Children = BuildMenuTree(menus, m.Id),
                Buttons = m.Buttons.Select(b => new ButtonDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    MenuId = b.MenuId,
                    ButtonType = b.ButtonType,
                    Sort = b.Sort,
                    IsEnabled = b.IsEnabled
                }).ToList()
            })
            .OrderBy(m => m.Sort)
            .ToList();
    }
}

