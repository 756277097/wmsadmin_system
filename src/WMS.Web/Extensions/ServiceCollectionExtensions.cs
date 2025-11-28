using System.Reflection;
using WMS.Application.Services;

namespace WMS.Web.Extensions;

/// <summary>
/// 服务注册扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 自动注册所有服务（接口和实现类）
    /// 约定：接口以 I 开头，实现类去掉 I 前缀，例如 IUserService -> UserService
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 获取 Application 程序集
        var assembly = typeof(IAuthService).Assembly;
        
        // 获取所有服务接口（以 I 开头，以 Service 结尾）
        var serviceInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface 
                && t.Name.StartsWith("I") 
                && t.Name.EndsWith("Service")
                && t != typeof(IServiceProvider))
            .ToList();

        // 为每个接口找到对应的实现类并注册
        foreach (var serviceInterface in serviceInterfaces)
        {
            // 实现类名称：去掉 I 前缀
            var implementationName = serviceInterface.Name.Substring(1);
            
            // 查找实现类（在同一程序集中，实现该接口，且不是接口本身）
            var implementation = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass 
                    && !t.IsAbstract 
                    && t.Name == implementationName
                    && serviceInterface.IsAssignableFrom(t));

            if (implementation != null)
            {
                // 注册为 Scoped 生命周期
                services.AddScoped(serviceInterface, implementation);
                Console.WriteLine($"✓ 自动注册服务: {serviceInterface.Name} -> {implementation.Name}");
            }
            else
            {
                Console.WriteLine($"⚠️  未找到实现类: {serviceInterface.Name} (期望: {implementationName})");
            }
        }

        return services;
    }
}

