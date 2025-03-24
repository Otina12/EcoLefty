using Common.Extensions;
using EcoLefty.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureContext(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureIdentity();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
//.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// seed data
//await EcoLeftySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
