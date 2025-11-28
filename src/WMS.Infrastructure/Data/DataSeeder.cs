using System.Security.Cryptography;
using System.Text;
using WMS.Core.Entities;

namespace WMS.Infrastructure.Data;

/// <summary>
/// 数据种子服务
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// 初始化基础数据
    /// </summary>
    public static async Task SeedAsync(WmsDbContext context)
    {
        // 检查是否已初始化
        if (context.Users.Any())
        {
            return; // 已初始化，跳过
        }

        // 创建超级管理员角色
        var superAdminRole = new Role
        {
            Name = "超级管理员",
            Code = "SuperAdmin",
            Description = "拥有所有权限",
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Roles.Add(superAdminRole);

        // 创建普通管理员角色
        var adminRole = new Role
        {
            Name = "普通管理员",
            Code = "Admin",
            Description = "普通管理权限",
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Roles.Add(adminRole);

        await context.SaveChangesAsync();

        // 创建管理员用户（密码：admin123）
        var adminUser = new User
        {
            UserName = "admin",
            Password = HashPassword("admin123"),
            RealName = "系统管理员",
            Email = "admin@wms.com",
            Phone = "13800138000",
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Users.Add(adminUser);

        // 创建测试用户（密码：user123）
        var testUser = new User
        {
            UserName = "test",
            Password = HashPassword("user123"),
            RealName = "测试用户",
            Email = "test@wms.com",
            Phone = "13800138001",
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Users.Add(testUser);

        // 创建普通用户（密码：user123）
        var normalUser = new User
        {
            UserName = "user",
            Password = HashPassword("user123"),
            RealName = "普通用户",
            Email = "user@wms.com",
            Phone = "13800138002",
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Users.Add(normalUser);

        await context.SaveChangesAsync();

        // 分配角色
        context.UserRoles.Add(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = superAdminRole.Id,
            CreateTime = DateTime.Now
        });

        context.UserRoles.Add(new UserRole
        {
            UserId = testUser.Id,
            RoleId = adminRole.Id,
            CreateTime = DateTime.Now
        });

        context.UserRoles.Add(new UserRole
        {
            UserId = normalUser.Id,
            RoleId = adminRole.Id,
            CreateTime = DateTime.Now
        });

        // 创建基础菜单
        var systemMenu = new Menu
        {
            Name = "系统管理",
            Code = "System",
            ParentId = null,
            MenuType = 0,
            Path = "#",
            Icon = "⚙️",
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(systemMenu);

        var userMenu = new Menu
        {
            Name = "用户管理",
            Code = "User",
            ParentId = systemMenu.Id,
            MenuType = 0,
            Path = "/User",
            Icon = "👤",
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(userMenu);

        var roleMenu = new Menu
        {
            Name = "角色管理",
            Code = "Role",
            ParentId = systemMenu.Id,
            MenuType = 0,
            Path = "/Role",
            Icon = "🔐",
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(roleMenu);

        var menuMenu = new Menu
        {
            Name = "菜单管理",
            Code = "Menu",
            ParentId = systemMenu.Id,
            MenuType = 0,
            Path = "/Menu",
            Icon = "📋",
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(menuMenu);

        // 创建业务菜单示例
        var businessMenu = new Menu
        {
            Name = "业务管理",
            Code = "Business",
            ParentId = null,
            MenuType = 0,
            Path = "#",
            Icon = "📦",
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(businessMenu);

        var warehouseMenu = new Menu
        {
            Name = "仓库管理",
            Code = "Warehouse",
            ParentId = businessMenu.Id,
            MenuType = 0,
            Path = "/Warehouse",
            Icon = "🏭",
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(warehouseMenu);

        var inventoryMenu = new Menu
        {
            Name = "库存管理",
            Code = "Inventory",
            ParentId = businessMenu.Id,
            MenuType = 0,
            Path = "/Inventory",
            Icon = "📊",
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(inventoryMenu);

        // 创建报表菜单
        var reportMenu = new Menu
        {
            Name = "报表中心",
            Code = "Report",
            ParentId = null,
            MenuType = 0,
            Path = "#",
            Icon = "📈",
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(reportMenu);

        var dailyReportMenu = new Menu
        {
            Name = "日报表",
            Code = "DailyReport",
            ParentId = reportMenu.Id,
            MenuType = 0,
            Path = "/Report/Daily",
            Icon = "📅",
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(dailyReportMenu);

        var monthlyReportMenu = new Menu
        {
            Name = "月报表",
            Code = "MonthlyReport",
            ParentId = reportMenu.Id,
            MenuType = 0,
            Path = "/Report/Monthly",
            Icon = "📆",
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Menus.Add(monthlyReportMenu);

        await context.SaveChangesAsync();

        // 为用户管理菜单添加按钮
        var userAddButton = new Button
        {
            Name = "新增",
            Code = "User:Add",
            MenuId = userMenu.Id,
            ButtonType = 1,
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(userAddButton);

        var userEditButton = new Button
        {
            Name = "编辑",
            Code = "User:Edit",
            MenuId = userMenu.Id,
            ButtonType = 2,
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(userEditButton);

        var userDeleteButton = new Button
        {
            Name = "删除",
            Code = "User:Delete",
            MenuId = userMenu.Id,
            ButtonType = 3,
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(userDeleteButton);

        var userViewButton = new Button
        {
            Name = "查看",
            Code = "User:View",
            MenuId = userMenu.Id,
            ButtonType = 0,
            Sort = 0,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(userViewButton);

        // 为角色管理菜单添加按钮
        var roleAddButton = new Button
        {
            Name = "新增",
            Code = "Role:Add",
            MenuId = roleMenu.Id,
            ButtonType = 1,
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(roleAddButton);

        var roleEditButton = new Button
        {
            Name = "编辑",
            Code = "Role:Edit",
            MenuId = roleMenu.Id,
            ButtonType = 2,
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(roleEditButton);

        var roleDeleteButton = new Button
        {
            Name = "删除",
            Code = "Role:Delete",
            MenuId = roleMenu.Id,
            ButtonType = 3,
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(roleDeleteButton);

        // 为菜单管理菜单添加按钮
        var menuAddButton = new Button
        {
            Name = "新增",
            Code = "Menu:Add",
            MenuId = menuMenu.Id,
            ButtonType = 1,
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(menuAddButton);

        var menuEditButton = new Button
        {
            Name = "编辑",
            Code = "Menu:Edit",
            MenuId = menuMenu.Id,
            ButtonType = 2,
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(menuEditButton);

        var menuDeleteButton = new Button
        {
            Name = "删除",
            Code = "Menu:Delete",
            MenuId = menuMenu.Id,
            ButtonType = 3,
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(menuDeleteButton);

        // 为仓库管理菜单添加按钮
        var warehouseAddButton = new Button
        {
            Name = "新增",
            Code = "Warehouse:Add",
            MenuId = warehouseMenu.Id,
            ButtonType = 1,
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(warehouseAddButton);

        var warehouseEditButton = new Button
        {
            Name = "编辑",
            Code = "Warehouse:Edit",
            MenuId = warehouseMenu.Id,
            ButtonType = 2,
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(warehouseEditButton);

        var warehouseDeleteButton = new Button
        {
            Name = "删除",
            Code = "Warehouse:Delete",
            MenuId = warehouseMenu.Id,
            ButtonType = 3,
            Sort = 3,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(warehouseDeleteButton);

        // 为库存管理菜单添加按钮
        var inventoryViewButton = new Button
        {
            Name = "查看",
            Code = "Inventory:View",
            MenuId = inventoryMenu.Id,
            ButtonType = 0,
            Sort = 1,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(inventoryViewButton);

        var inventoryExportButton = new Button
        {
            Name = "导出",
            Code = "Inventory:Export",
            MenuId = inventoryMenu.Id,
            ButtonType = 4,
            Sort = 2,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        context.Buttons.Add(inventoryExportButton);

        // 为超级管理员分配所有权限
        var allMenus = context.Menus.ToList();
        var allButtons = context.Buttons.ToList();

        foreach (var menu in allMenus)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                MenuId = menu.Id,
                PermissionType = 0,
                CreateTime = DateTime.Now
            });
        }

        foreach (var button in allButtons)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                MenuId = button.MenuId,
                ButtonId = button.Id,
                PermissionType = 1,
                CreateTime = DateTime.Now
            });
        }

        // 为普通管理员分配部分权限（只有用户管理和角色管理的查看权限）
        var adminMenus = new[] { systemMenu, userMenu, roleMenu };
        foreach (var menu in adminMenus)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                MenuId = menu.Id,
                PermissionType = 0,
                CreateTime = DateTime.Now
            });
        }

        // 普通管理员只有查看和编辑权限，没有删除权限
        var adminButtons = allButtons.Where(b => 
            (b.Code.Contains("User:") || b.Code.Contains("Role:")) && 
            !b.Code.Contains("Delete")).ToList();
        
        foreach (var button in adminButtons)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                MenuId = button.MenuId,
                ButtonId = button.Id,
                PermissionType = 1,
                CreateTime = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

