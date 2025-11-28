using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using WMS.Core.Entities;
using WMS.Infrastructure.Data;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("错误：未找到数据库连接字符串！");
    Console.WriteLine("请在 appsettings.json 中配置 DefaultConnection");
    return;
}

var optionsBuilder = new DbContextOptionsBuilder<WmsDbContext>();
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

using var context = new WmsDbContext(optionsBuilder.Options);

Console.WriteLine("开始初始化种子数据...");
Console.WriteLine($"数据库连接：{connectionString.Split(';').FirstOrDefault(s => s.StartsWith("Database"))}");

try
{
    // 检查数据库连接
    try
    {
        await context.Database.CanConnectAsync();
        Console.WriteLine("✓ 数据库连接成功");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ 数据库连接失败：{ex.Message}");
        return;
    }

    // 检查并修复 ParentId 列（允许 NULL）
    try
    {
        Console.WriteLine("\n检查并修复数据库结构...");
        
        // 直接尝试修改列结构（如果已经是可空的，MySQL 不会报错）
        try
        {
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE Menus MODIFY COLUMN ParentId INT NULL");
            Console.WriteLine("✓ 已更新 ParentId 列为可空类型");
        }
        catch (Exception ex)
        {
            // 如果列不存在或其他错误，提示用户
            if (ex.Message.Contains("doesn't exist") || ex.Message.Contains("Unknown column"))
            {
                Console.WriteLine("⚠️  表或列不存在，将尝试创建表结构");
            }
            else
            {
                // 其他错误可能是列已经是可空的，忽略
                Console.WriteLine($"   提示：{ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  修复数据库结构时出现警告：{ex.Message}");
        Console.WriteLine("   如果后续插入失败，请手动执行以下 SQL：");
        Console.WriteLine("   ALTER TABLE Menus MODIFY COLUMN ParentId INT NULL;");
    }


    // 检查是否已初始化
    var hasUsers = await context.Users.AnyAsync();
    var hasMenus = await context.Menus.AnyAsync();
    
    if (hasUsers || hasMenus)
    {
        Console.WriteLine($"\n检测到数据库中已有数据：");
        Console.WriteLine($"  用户数量：{await context.Users.CountAsync()}");
        Console.WriteLine($"  菜单数量：{await context.Menus.CountAsync()}");
        Console.WriteLine("\n选项：");
        Console.WriteLine("  1. 清理所有数据后重新初始化 (推荐)");
        Console.WriteLine("  2. 跳过已存在的数据，只插入新数据");
        Console.WriteLine("  3. 取消操作");
        Console.Write("\n请选择 (1/2/3): ");
        var input = Console.ReadLine();
        
        if (input == "1")
        {
            Console.WriteLine("\n正在清理数据...");
            try
            {
                // 按依赖关系顺序删除
                await context.Database.ExecuteSqlRawAsync("DELETE FROM RolePermissions");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM UserRoles");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Buttons");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Menus");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Roles");
                Console.WriteLine("✓ 数据清理完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清理数据时出错：{ex.Message}");
                Console.WriteLine("请手动执行以下SQL清理数据：");
                Console.WriteLine("DELETE FROM RolePermissions;");
                Console.WriteLine("DELETE FROM UserRoles;");
                Console.WriteLine("DELETE FROM Buttons;");
                Console.WriteLine("DELETE FROM Menus;");
                Console.WriteLine("DELETE FROM Users;");
                Console.WriteLine("DELETE FROM Roles;");
                return;
            }
        }
        else if (input == "2")
        {
            Console.WriteLine("将跳过已存在的数据...");
        }
        else
        {
            Console.WriteLine("已取消操作");
            return;
        }
    }

    // 不使用事务，分阶段提交，避免外键约束问题
    try
    {
        // 1. 创建角色
        Console.WriteLine("\n1. 创建角色...");
        var roles = await SeedRolesAsync(context);
        Console.WriteLine($"   已创建 {roles.Count} 个角色");

        // 2. 创建用户
        Console.WriteLine("\n2. 创建用户...");
        var users = await SeedUsersAsync(context);
        Console.WriteLine($"   已创建 {users.Count} 个用户");

        // 3. 创建菜单（分阶段：先父后子）
        Console.WriteLine("\n3. 创建菜单...");
        var menus = await SeedMenusAsync(context);
        Console.WriteLine($"   已创建 {menus.Count} 个菜单");

        // 4. 创建按钮
        Console.WriteLine("\n4. 创建按钮...");
        var buttons = await SeedButtonsAsync(context, menus);
        Console.WriteLine($"   已创建 {buttons.Count} 个按钮");

        // 5. 分配用户角色
        Console.WriteLine("\n5. 分配用户角色...");
        await SeedUserRolesAsync(context, users, roles);
        Console.WriteLine("   用户角色分配完成");

        // 6. 分配角色权限
        Console.WriteLine("\n6. 分配角色权限...");
        await SeedRolePermissionsAsync(context, roles, menus, buttons);
        Console.WriteLine("   角色权限分配完成");

        Console.WriteLine("\n✅ 种子数据初始化完成！");
        Console.WriteLine("\n默认账户信息：");
        Console.WriteLine("  用户名：admin");
        Console.WriteLine("  密码：admin123");
        Console.WriteLine("  角色：超级管理员");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 初始化失败：{ex.Message}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   内部错误：{ex.InnerException.Message}");
        }
        
        // 如果是外键约束错误，提供更详细的提示
        if (ex.Message.Contains("foreign key constraint") || ex.InnerException?.Message.Contains("foreign key constraint") == true)
        {
            Console.WriteLine("\n💡 提示：");
            Console.WriteLine("   这可能是由于数据库中已有不完整的数据导致的。");
            Console.WriteLine("   建议执行以下SQL清理数据：");
            Console.WriteLine("   DELETE FROM RolePermissions;");
            Console.WriteLine("   DELETE FROM UserRoles;");
            Console.WriteLine("   DELETE FROM Buttons;");
            Console.WriteLine("   DELETE FROM Menus;");
            Console.WriteLine("   DELETE FROM Users;");
            Console.WriteLine("   DELETE FROM Roles;");
        }
        
        Console.WriteLine($"\n   详细堆栈：{ex}");
        throw;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ 发生错误：{ex.Message}");
    Console.WriteLine($"   详细错误：{ex}");
}

static async Task<Dictionary<string, Role>> SeedRolesAsync(WmsDbContext context)
{
    var roles = new List<Role>
    {
        new Role { Name = "超级管理员", Code = "SuperAdmin", Description = "拥有所有权限", IsEnabled = true, CreateTime = DateTime.Now },
        new Role { Name = "普通管理员", Code = "Admin", Description = "普通管理权限，可管理用户和角色", IsEnabled = true, CreateTime = DateTime.Now },
        new Role { Name = "仓库管理员", Code = "WarehouseManager", Description = "负责仓库日常管理", IsEnabled = true, CreateTime = DateTime.Now },
        new Role { Name = "库存管理员", Code = "InventoryManager", Description = "负责库存管理", IsEnabled = true, CreateTime = DateTime.Now },
        new Role { Name = "只读用户", Code = "ReadOnly", Description = "只能查看，无操作权限", IsEnabled = true, CreateTime = DateTime.Now }
    };

    var roleDict = new Dictionary<string, Role>();

    foreach (var role in roles)
    {
        var existing = await context.Roles.FirstOrDefaultAsync(r => r.Code == role.Code);
        if (existing == null)
        {
            context.Roles.Add(role);
            await context.SaveChangesAsync();
            roleDict[role.Code] = role;
            Console.WriteLine($"   ✓ {role.Name} ({role.Code})");
        }
        else
        {
            roleDict[role.Code] = existing;
            Console.WriteLine($"   - {role.Name} ({role.Code}) [已存在]");
        }
    }

    return roleDict;
}

static async Task<Dictionary<string, User>> SeedUsersAsync(WmsDbContext context)
{
    var users = new List<User>
    {
        new User { UserName = "admin", Password = HashPassword("admin123"), RealName = "系统管理员", Email = "admin@wms.com", Phone = "13800138000", IsEnabled = true, CreateTime = DateTime.Now },
        new User { UserName = "test", Password = HashPassword("admin123"), RealName = "测试用户", Email = "test@wms.com", Phone = "13800138001", IsEnabled = true, CreateTime = DateTime.Now },
        new User { UserName = "user", Password = HashPassword("admin123"), RealName = "普通用户", Email = "user@wms.com", Phone = "13800138002", IsEnabled = true, CreateTime = DateTime.Now },
        new User { UserName = "warehouse", Password = HashPassword("admin123"), RealName = "仓库管理员", Email = "warehouse@wms.com", Phone = "13800138003", IsEnabled = true, CreateTime = DateTime.Now },
        new User { UserName = "inventory", Password = HashPassword("admin123"), RealName = "库存管理员", Email = "inventory@wms.com", Phone = "13800138004", IsEnabled = true, CreateTime = DateTime.Now }
    };

    var userDict = new Dictionary<string, User>();

    foreach (var user in users)
    {
        var existing = await context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
        if (existing == null)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            userDict[user.UserName] = user;
            Console.WriteLine($"   ✓ {user.UserName} ({user.RealName})");
        }
        else
        {
            userDict[user.UserName] = existing;
            Console.WriteLine($"   - {user.UserName} ({user.RealName}) [已存在]");
        }
    }

    return userDict;
}

static async Task<Dictionary<string, Menu>> SeedMenusAsync(WmsDbContext context)
{
    var menus = new List<Menu>();

    // 顶级菜单（ParentId 使用 null 而不是 0，避免外键约束问题）
    var systemMenu = new Menu { Name = "系统管理", Code = "System", ParentId = null, MenuType = 0, Path = "#", Icon = "⚙️", Sort = 1, IsEnabled = true, CreateTime = DateTime.Now };
    var businessMenu = new Menu { Name = "业务管理", Code = "Business", ParentId = null, MenuType = 0, Path = "#", Icon = "📦", Sort = 2, IsEnabled = true, CreateTime = DateTime.Now };
    var reportMenu = new Menu { Name = "报表中心", Code = "Report", ParentId = null, MenuType = 0, Path = "#", Icon = "📈", Sort = 3, IsEnabled = true, CreateTime = DateTime.Now };

    menus.Add(systemMenu);
    menus.Add(businessMenu);
    menus.Add(reportMenu);

    var menuDict = new Dictionary<string, Menu>();

    // 先插入顶级菜单（ParentId = 0）
    // 注意：如果外键约束不允许ParentId=0，需要先禁用外键检查或使用NULL
    foreach (var menu in menus)
    {
        var existing = await context.Menus.FirstOrDefaultAsync(m => m.Code == menu.Code);
        if (existing == null)
        {
            try
            {
                context.Menus.Add(menu);
                await context.SaveChangesAsync();
                
                // 强制刷新，确保ID已生成
                await context.Entry(menu).ReloadAsync();
                
                // 从数据库重新查询以确保获取正确的ID
                var saved = await context.Menus.FirstOrDefaultAsync(m => m.Code == menu.Code);
                if (saved == null || saved.Id <= 0)
                {
                    throw new Exception($"菜单 {menu.Name} ({menu.Code}) 保存失败，无法获取ID");
                }
                
                menuDict[menu.Code] = saved;
                Console.WriteLine($"   ✓ {menu.Name} ({menu.Code}) [ID: {saved.Id}, ParentId: {saved.ParentId}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 插入菜单失败：{menu.Name} ({menu.Code})");
                Console.WriteLine($"      错误：{ex.Message}");
                
                // 如果是外键约束错误，可能是ParentId=0的问题
                if (ex.Message.Contains("foreign key"))
                {
                    Console.WriteLine($"      提示：可能是ParentId=0的外键约束问题");
                    Console.WriteLine($"      建议：检查数据库外键约束是否允许ParentId=0");
                }
                throw;
            }
        }
        else
        {
            menuDict[menu.Code] = existing;
            Console.WriteLine($"   - {menu.Name} ({menu.Code}) [已存在, ID: {existing.Id}, ParentId: {existing.ParentId}]");
        }
    }

    // 验证父菜单是否存在并从数据库重新查询以确保ID正确
    var systemMenuFromDb = await context.Menus.FirstOrDefaultAsync(m => m.Code == "System");
    var businessMenuFromDb = await context.Menus.FirstOrDefaultAsync(m => m.Code == "Business");
    var reportMenuFromDb = await context.Menus.FirstOrDefaultAsync(m => m.Code == "Report");

    if (systemMenuFromDb == null || systemMenuFromDb.Id <= 0)
        throw new Exception("系统管理菜单不存在或ID无效");
    if (businessMenuFromDb == null || businessMenuFromDb.Id <= 0)
        throw new Exception("业务管理菜单不存在或ID无效");
    if (reportMenuFromDb == null || reportMenuFromDb.Id <= 0)
        throw new Exception("报表中心菜单不存在或ID无效");

    var systemParentId = systemMenuFromDb.Id;
    var businessParentId = businessMenuFromDb.Id;
    var reportParentId = reportMenuFromDb.Id;

    Console.WriteLine($"   父菜单ID - System: {systemParentId}, Business: {businessParentId}, Report: {reportParentId}");

    // 系统管理子菜单
    var systemSubMenus = new List<Menu>
    {
        new Menu { Name = "用户管理", Code = "User", ParentId = systemParentId, MenuType = 0, Path = "/User", Icon = "👤", Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "角色管理", Code = "Role", ParentId = systemParentId, MenuType = 0, Path = "/Role", Icon = "🔐", Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "菜单管理", Code = "Menu", ParentId = systemParentId, MenuType = 0, Path = "/Menu", Icon = "📋", Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    };

    // 业务管理子菜单
    var businessSubMenus = new List<Menu>
    {
        new Menu { Name = "物料管理", Code = "Material", ParentId = businessParentId, MenuType = 0, Path = "/Material", Icon = "📦", Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "仓库管理", Code = "Warehouse", ParentId = businessParentId, MenuType = 0, Path = "/Warehouse", Icon = "🏭", Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "库存管理", Code = "Inventory", ParentId = businessParentId, MenuType = 0, Path = "/Inventory", Icon = "📊", Sort = 3, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "入库管理", Code = "Inbound", ParentId = businessParentId, MenuType = 0, Path = "/Inbound", Icon = "📥", Sort = 4, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "出库管理", Code = "Outbound", ParentId = businessParentId, MenuType = 0, Path = "/Outbound", Icon = "📤", Sort = 5, IsEnabled = true, CreateTime = DateTime.Now }
    };

    // 报表中心子菜单
    var reportSubMenus = new List<Menu>
    {
        new Menu { Name = "日报表", Code = "DailyReport", ParentId = reportParentId, MenuType = 0, Path = "/Report/Daily", Icon = "📅", Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "月报表", Code = "MonthlyReport", ParentId = reportParentId, MenuType = 0, Path = "/Report/Monthly", Icon = "📆", Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Menu { Name = "库存报表", Code = "InventoryReport", ParentId = reportParentId, MenuType = 0, Path = "/Report/Inventory", Icon = "📊", Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    };

    // 插入子菜单
    foreach (var menu in systemSubMenus.Concat(businessSubMenus).Concat(reportSubMenus))
    {
        // 验证父菜单是否存在（从数据库查询）
        var parentMenu = await context.Menus.FirstOrDefaultAsync(m => m.Id == menu.ParentId);
        if (parentMenu == null)
        {
            // 列出所有菜单以便调试
            var allMenus = await context.Menus.Select(m => new { m.Id, m.Code, m.Name, m.ParentId }).ToListAsync();
            Console.WriteLine($"\n   ❌ 父菜单不存在！ParentId: {menu.ParentId}, 菜单: {menu.Name} ({menu.Code})");
            Console.WriteLine($"   当前数据库中的菜单：");
            foreach (var m in allMenus)
            {
                Console.WriteLine($"      ID: {m.Id}, Code: {m.Code}, Name: {m.Name}, ParentId: {m.ParentId}");
            }
            throw new Exception($"父菜单不存在！ParentId: {menu.ParentId}, 菜单: {menu.Name} ({menu.Code})");
        }

        var existing = await context.Menus.FirstOrDefaultAsync(m => m.Code == menu.Code);
        if (existing == null)
        {
            try
            {
                // 再次确认父菜单ID
                menu.ParentId = parentMenu.Id;
                
                context.Menus.Add(menu);
                await context.SaveChangesAsync();
                
                // 强制刷新
                await context.Entry(menu).ReloadAsync();
                
                // 从数据库重新查询以确保获取正确的ID
                var saved = await context.Menus.FirstOrDefaultAsync(m => m.Code == menu.Code);
                if (saved == null)
                    throw new Exception($"菜单 {menu.Name} ({menu.Code}) 保存后无法查询到");
                    
                menuDict[menu.Code] = saved;
                Console.WriteLine($"   ✓ {menu.Name} ({menu.Code}) [ID: {saved.Id}, ParentId: {saved.ParentId}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 插入菜单失败：{menu.Name} ({menu.Code}), ParentId: {menu.ParentId}");
                Console.WriteLine($"      错误：{ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"      内部错误：{ex.InnerException.Message}");
                }
                throw;
            }
        }
        else
        {
            menuDict[menu.Code] = existing;
            Console.WriteLine($"   - {menu.Name} ({menu.Code}) [已存在, ID: {existing.Id}, ParentId: {existing.ParentId}]");
        }
    }

    return menuDict;
}

static async Task<Dictionary<string, Button>> SeedButtonsAsync(WmsDbContext context, Dictionary<string, Menu> menus)
{
    var buttons = new List<Button>();

    // 用户管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "User:View", MenuId = menus["User"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "新增", Code = "User:Add", MenuId = menus["User"].Id, ButtonType = 1, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "编辑", Code = "User:Edit", MenuId = menus["User"].Id, ButtonType = 2, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "删除", Code = "User:Delete", MenuId = menus["User"].Id, ButtonType = 3, Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    });

    // 角色管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "Role:View", MenuId = menus["Role"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "新增", Code = "Role:Add", MenuId = menus["Role"].Id, ButtonType = 1, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "编辑", Code = "Role:Edit", MenuId = menus["Role"].Id, ButtonType = 2, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "删除", Code = "Role:Delete", MenuId = menus["Role"].Id, ButtonType = 3, Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    });

    // 菜单管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "Menu:View", MenuId = menus["Menu"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "新增", Code = "Menu:Add", MenuId = menus["Menu"].Id, ButtonType = 1, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "编辑", Code = "Menu:Edit", MenuId = menus["Menu"].Id, ButtonType = 2, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "删除", Code = "Menu:Delete", MenuId = menus["Menu"].Id, ButtonType = 3, Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    });

    // 物料管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "Material:View", MenuId = menus["Material"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "新增", Code = "Material:Add", MenuId = menus["Material"].Id, ButtonType = 1, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "编辑", Code = "Material:Edit", MenuId = menus["Material"].Id, ButtonType = 2, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "删除", Code = "Material:Delete", MenuId = menus["Material"].Id, ButtonType = 3, Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    });

    // 仓库管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "Warehouse:View", MenuId = menus["Warehouse"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "新增", Code = "Warehouse:Add", MenuId = menus["Warehouse"].Id, ButtonType = 1, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "编辑", Code = "Warehouse:Edit", MenuId = menus["Warehouse"].Id, ButtonType = 2, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "删除", Code = "Warehouse:Delete", MenuId = menus["Warehouse"].Id, ButtonType = 3, Sort = 3, IsEnabled = true, CreateTime = DateTime.Now }
    });

    // 库存管理按钮
    buttons.AddRange(new[]
    {
        new Button { Name = "查看", Code = "Inventory:View", MenuId = menus["Inventory"].Id, ButtonType = 0, Sort = 0, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "导出", Code = "Inventory:Export", MenuId = menus["Inventory"].Id, ButtonType = 4, Sort = 1, IsEnabled = true, CreateTime = DateTime.Now },
        new Button { Name = "盘点", Code = "Inventory:Stocktake", MenuId = menus["Inventory"].Id, ButtonType = 4, Sort = 2, IsEnabled = true, CreateTime = DateTime.Now }
    });

    var buttonDict = new Dictionary<string, Button>();

    foreach (var button in buttons)
    {
        var existing = await context.Buttons.FirstOrDefaultAsync(b => b.Code == button.Code);
        if (existing == null)
        {
            context.Buttons.Add(button);
            await context.SaveChangesAsync();
            buttonDict[button.Code] = button;
            Console.WriteLine($"   ✓ {button.Name} ({button.Code})");
        }
        else
        {
            buttonDict[button.Code] = existing;
            Console.WriteLine($"   - {button.Name} ({button.Code}) [已存在]");
        }
    }

    return buttonDict;
}

static async Task SeedUserRolesAsync(WmsDbContext context, Dictionary<string, User> users, Dictionary<string, Role> roles)
{
    var userRoles = new List<(string UserName, string RoleCode)>
    {
        ("admin", "SuperAdmin"),
        ("test", "Admin"),
        ("user", "Admin"),
        ("warehouse", "WarehouseManager"),
        ("inventory", "InventoryManager")
    };

    foreach (var (userName, roleCode) in userRoles)
    {
        if (!users.ContainsKey(userName) || !roles.ContainsKey(roleCode))
            continue;

        var user = users[userName];
        var role = roles[roleCode];

        var existing = await context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

        if (existing == null)
        {
            context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                CreateTime = DateTime.Now
            });
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✓ {userName} -> {role.Name}");
        }
        else
        {
            Console.WriteLine($"   - {userName} -> {role.Name} [已存在]");
        }
    }
}

static async Task SeedRolePermissionsAsync(WmsDbContext context, Dictionary<string, Role> roles, Dictionary<string, Menu> menus, Dictionary<string, Button> buttons)
{
    // 超级管理员 - 所有权限
    var superAdmin = roles["SuperAdmin"];
    var allMenus = menus.Values.ToList();
    var allButtons = buttons.Values.ToList();

    foreach (var menu in allMenus)
    {
        var existing = await context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == superAdmin.Id && rp.MenuId == menu.Id && rp.PermissionType == 0);
        if (existing == null)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdmin.Id,
                MenuId = menu.Id,
                PermissionType = 0,
                CreateTime = DateTime.Now
            });
        }
    }

    foreach (var button in allButtons)
    {
        var existing = await context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == superAdmin.Id && rp.ButtonId == button.Id && rp.PermissionType == 1);
        if (existing == null)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdmin.Id,
                MenuId = button.MenuId,
                ButtonId = button.Id,
                PermissionType = 1,
                CreateTime = DateTime.Now
            });
        }
    }

    await context.SaveChangesAsync();
    Console.WriteLine($"   ✓ 超级管理员：{allMenus.Count} 个菜单，{allButtons.Count} 个按钮");

    // 普通管理员 - 系统管理权限
    if (roles.ContainsKey("Admin"))
    {
        var admin = roles["Admin"];
        var adminMenus = new[] { menus["System"], menus["User"], menus["Role"] };
        var adminButtons = buttons.Values.Where(b => (b.Code.StartsWith("User:") || b.Code.StartsWith("Role:")) && !b.Code.Contains("Delete")).ToList();

        foreach (var menu in adminMenus)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == admin.Id && rp.MenuId == menu.Id && rp.PermissionType == 0);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = admin.Id,
                    MenuId = menu.Id,
                    PermissionType = 0,
                    CreateTime = DateTime.Now
                });
            }
        }

        foreach (var button in adminButtons)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == admin.Id && rp.ButtonId == button.Id && rp.PermissionType == 1);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = admin.Id,
                    MenuId = button.MenuId,
                    ButtonId = button.Id,
                    PermissionType = 1,
                    CreateTime = DateTime.Now
                });
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"   ✓ 普通管理员：{adminMenus.Length} 个菜单，{adminButtons.Count} 个按钮");
    }

    // 仓库管理员
    if (roles.ContainsKey("WarehouseManager") && menus.ContainsKey("Warehouse"))
    {
        var warehouseManager = roles["WarehouseManager"];
        var warehouseMenus = new[] { menus["Business"], menus["Warehouse"], menus["Inbound"], menus["Outbound"] };
        var warehouseButtons = buttons.Values.Where(b => b.Code.StartsWith("Warehouse:")).ToList();

        foreach (var menu in warehouseMenus)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == warehouseManager.Id && rp.MenuId == menu.Id && rp.PermissionType == 0);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = warehouseManager.Id,
                    MenuId = menu.Id,
                    PermissionType = 0,
                    CreateTime = DateTime.Now
                });
            }
        }

        foreach (var button in warehouseButtons)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == warehouseManager.Id && rp.ButtonId == button.Id && rp.PermissionType == 1);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = warehouseManager.Id,
                    MenuId = button.MenuId,
                    ButtonId = button.Id,
                    PermissionType = 1,
                    CreateTime = DateTime.Now
                });
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"   ✓ 仓库管理员：{warehouseMenus.Length} 个菜单，{warehouseButtons.Count} 个按钮");
    }

    // 库存管理员
    if (roles.ContainsKey("InventoryManager"))
    {
        var inventoryManager = roles["InventoryManager"];
        var inventoryMenus = new[] { menus["Business"], menus["Inventory"], menus["Report"], menus["InventoryReport"] };
        var inventoryButtons = buttons.Values.Where(b => b.Code.StartsWith("Inventory:")).ToList();

        foreach (var menu in inventoryMenus)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == inventoryManager.Id && rp.MenuId == menu.Id && rp.PermissionType == 0);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = inventoryManager.Id,
                    MenuId = menu.Id,
                    PermissionType = 0,
                    CreateTime = DateTime.Now
                });
            }
        }

        foreach (var button in inventoryButtons)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == inventoryManager.Id && rp.ButtonId == button.Id && rp.PermissionType == 1);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = inventoryManager.Id,
                    MenuId = button.MenuId,
                    ButtonId = button.Id,
                    PermissionType = 1,
                    CreateTime = DateTime.Now
                });
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"   ✓ 库存管理员：{inventoryMenus.Length} 个菜单，{inventoryButtons.Count} 个按钮");
    }

    // 只读用户
    if (roles.ContainsKey("ReadOnly"))
    {
        var readOnly = roles["ReadOnly"];

        foreach (var menu in allMenus)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == readOnly.Id && rp.MenuId == menu.Id && rp.PermissionType == 0);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = readOnly.Id,
                    MenuId = menu.Id,
                    PermissionType = 0,
                    CreateTime = DateTime.Now
                });
            }
        }

        var viewButtons = allButtons.Where(b => b.ButtonType == 0).ToList();
        foreach (var button in viewButtons)
        {
            var existing = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == readOnly.Id && rp.ButtonId == button.Id && rp.PermissionType == 1);
            if (existing == null)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = readOnly.Id,
                    MenuId = button.MenuId,
                    ButtonId = button.Id,
                    PermissionType = 1,
                    CreateTime = DateTime.Now
                });
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"   ✓ 只读用户：{allMenus.Count} 个菜单，{viewButtons.Count} 个查看按钮");
    }
}

static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}

