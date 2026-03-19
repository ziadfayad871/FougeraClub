using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FougeraClub.Application.Interfaces.Repositories;
using FougeraClub.Application.Interfaces.Services;
using FougeraClub.Application.Mappings;
using FougeraClub.Application.Services;
using FougeraClub.Infrastructure.Persistence;
using FougeraClub.Infrastructure.Repositories;
using FougeraClub.Web.Notifications;
using FougeraClub.Web.Otp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var sqlConnectionString = builder.Configuration.GetConnectionString("default");
if (string.IsNullOrWhiteSpace(sqlConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings:default is required.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(sqlConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(60);
    });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.Configure<ManagerOtpOptions>(builder.Configuration.GetSection("ManagerOtp"));
builder.Services.AddScoped<IManagerOtpService, ManagerOtpService>();
builder.Services.AddScoped<IOtpSender, NoopOtpSender>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<INotificationStore, InMemoryNotificationStore>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    await DbInitializer.SeedAsync(db, logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowAll");

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapRazorPages();
app.Run();
