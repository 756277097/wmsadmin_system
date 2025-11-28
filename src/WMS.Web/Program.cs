using Microsoft.EntityFrameworkCore;
using WMS.Application.Services;
using WMS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllersWithViews(options =>
    {
        // 添加全局过滤器 - 确保每个页面都从Session加载最新权限
        options.Filters.Add<WMS.Web.Filters.RefreshPermissionFilter>();
    })
    .AddJsonOptions(options =>
    {
        // 配置 JSON 序列化为 camelCase，与前端 JavaScript 保持一致
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 配置数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WmsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 注册服务
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// 注册全局过滤器 - 确保每个页面都从Session加载最新权限
builder.Services.AddScoped<WMS.Web.Filters.RefreshPermissionFilter>();

// Session配置
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//// 初始化数据
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<WmsDbContext>();
//    try
//    {
//        await WMS.Infrastructure.Data.DataSeeder.SeedAsync(context);
//    }
//    catch (Exception ex)
//    {
//        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "初始化数据时发生错误");
//    }
//}

// 配置HTTP请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

