using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Helper.Email;
using RapidResuce.Interfaces;
using RapidResuce.Models;
using RapidResuce.Services;
using RapidResuce.SignalHub;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));

// Add SignalR
builder.Services.AddSignalR();

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection") ?? throw new InvalidOperationException("Connection string 'connection' not found.")));

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure Kestrel limits (optional)
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024; // 1 GB
});

// Add CORS policy (Ensure this is safe for your use case)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               //.AllowCredentials() // If you use AllowCredentials(), you can't use AllowAnyOrigin()
               .SetIsOriginAllowed((host) => true); // Allows all origins, safer than AllowAnyOrigin()
    });
});

var app = builder.Build();

// Configure middleware pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Enable CORS (should be used early in the pipeline)
app.UseCors("CorsPolicy");

// Enable WebSockets support
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    ReceiveBufferSize = 4 * 1024
});

// Enable static file serving
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// Enable session management
app.UseSession();

// Enable authorization (if you're using authentication/authorization)
app.UseAuthorization();

// Map SignalR hubs
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<LocationHub>("/locationHub");

// Map the default routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
