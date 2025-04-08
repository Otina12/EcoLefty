using Asp.Versioning;
using Common.Extensions;
using EcoLefty.API.Infrastructure;
using EcoLefty.API.Infrastructure.Extensions;
using EcoLefty.Application;
using EcoLefty.Workers;
using Quartz;
using Serilog;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.ConfigureSerilogILogger();
services.ConfigureLoggerService();
services.ConfigureHttpLogging();

services.AddExceptionHandler<ExceptionHandler>();

if (builder.Environment.IsDevelopment())
{
    services.ConfigureProblemDetails(); // more detailed problem details when in development
}
else
{
    services.AddProblemDetails();
}

services.ConfigureContext(configuration);
services.ConfigureServices();

services.ConfigureIdentity();

services.ConfigureHealthChecks(configuration);

builder.Services.AddQuartz(q =>
{
    q.AddOfferArchiverWorkerJob();
    q.AddHealthCheckWorkerJob();
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});


services.AddAutoMapper(typeof(MappingProfile));
services.AddControllers();
//.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
services.AddEndpointsApiExplorer();
services.ConfigureSwaggerGen();
services.ConfigureValidation();

services.ConfigureCors();

services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureJwtAuthentication(builder.Configuration);

services.AddAuthorization();


var app = builder.Build();

app.MapHealthChecks("health");

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseStatusCodePages();

// seed data
//await EcoLeftySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
