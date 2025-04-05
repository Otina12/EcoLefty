using Common.Extensions;
using EcoLefty.API.Infrastructure.Extensions;
using EcoLefty.Application;
using EcoLefty.Application.Common.Logger;
using EcoLefty.Web.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.ConfigureSerilogILogger();
services.ConfigureLoggerService();
services.ConfigureHttpLogging();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

services.ConfigureContext(configuration);
services.ConfigureServices();
services.ConfigureIdentity();
services.AddControllersWithViews();

services.ConfigureQuartz();
services.ConfigureHealthCheckWorker();

services.ConfigureValidators();
services.AddAutoMapper(typeof(MappingProfile));
services.ConfigureCors();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerService>();

app.UseSession();

app.ConfigureExceptionHandler(logger);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
