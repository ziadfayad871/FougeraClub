using FougeraClub.Application.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FougeraClub.Infrastructure.DependencyInjection;
using FougeraClub.Infrastructure.Persistence;
using FougeraClub.Web.DependencyInjection;
using FougeraClub.Web.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var sqlConnectionString = builder.Configuration.GetConnectionString("default");
var useSqliteFallback =
    !builder.Environment.IsDevelopment() &&
    (string.IsNullOrWhiteSpace(sqlConnectionString) ||
     sqlConnectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase));

string databaseProviderDescription;

if (useSqliteFallback)
{
    var appDataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(appDataDirectory);

    var sqliteDatabasePath = Path.Combine(appDataDirectory, "FougeraClub.db");
    var sqliteConnectionString = $"Data Source={sqliteDatabasePath}";

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));

    databaseProviderDescription = $"SQLite ({sqliteDatabasePath})";
}
else
{
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

    databaseProviderDescription = "SQL Server";
}

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

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddInfrastructureRepositories();
builder.Services.AddWebServices();

var app = builder.Build();
app.Logger.LogInformation("Using database provider: {DatabaseProvider}", databaseProviderDescription);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        await DbInitializer.SeedAsync(db, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed during startup. The app will continue running, but database-backed features may be unavailable.");
    }
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
app.Run();
