using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WMS.Application.Services;

namespace WMS.Web.Filters;

/// <summary>
/// 权限刷新过滤器 - 确保每个页面都从Session加载最新的权限数据
/// 登录后，每次页面加载时都会从数据库获取最新权限并更新到Session
/// </summary>
public class RefreshPermissionFilter : IAsyncActionFilter
{
    private readonly IAuthService _authService;

    public RefreshPermissionFilter(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 排除登录相关的Action，避免循环
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        
        // 跳过登录页面和API接口
        if (controllerName == "Auth" && (actionName == "Login" || actionName == "Logout"))
        {
            await next();
            return;
        }

        // 在执行Action之前，刷新Session中的权限数据
        var userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            try
            {
                // 从数据库获取最新的权限数据（确保权限变更后立即生效）
                var menus = await _authService.GetUserMenusAsync(userId.Value);
                var permissions = await _authService.GetUserPermissionsAsync(userId.Value);

                // 更新Session中的权限数据
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };
                context.HttpContext.Session.SetString("UserMenus", System.Text.Json.JsonSerializer.Serialize(menus, jsonOptions));
                context.HttpContext.Session.SetString("UserPermissions", System.Text.Json.JsonSerializer.Serialize(permissions, jsonOptions));
                
                // 调试日志（可选，生产环境可移除）
                // Console.WriteLine($"已刷新用户 {userId.Value} 的权限数据 - 菜单: {menus.Count}, 权限: {permissions.Count}");
            }
            catch (Exception ex)
            {
                // 如果刷新失败，记录错误但不阻止请求
                Console.WriteLine($"刷新权限数据失败: {ex.Message}");
            }
        }

        // 继续执行Action
        await next();
    }
}

