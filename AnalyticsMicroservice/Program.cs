using AnalyticsMicroservice.Contexts;
using AnalyticsMicroservice.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFilesFromDirectory(builder.Configuration.GetOrThrow("ConfigsDirectory"));

builder.Services.AddDbContext<ApplicationContext>(contextConfig => {
    contextConfig.UseMySQL(builder.Configuration.GetOrThrow("Analytics:Database:ConnectionString"));
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IMatrixService, MatrixService>();
builder.Services.AddScoped<IPriceService, PriceService>();

builder.Services.AddCors();
builder.Services.AddControllers();
builder.WebHost.UseUrls(builder.Configuration.GetOrThrow("Analytics:Url"));
builder.Services.AddSwaggerGen(swaggerGenConfig => {
    //swaggerGenConfig.SwaggerDoc("v1", new() {
    //    Title = "AnalyticsMicroservice",
    //    Version = "v1"
    //});
});

var app = builder.Build();

app.UseCors(corsConfig => {
    corsConfig.AllowAnyMethod();
    corsConfig.AllowAnyHeader();
    corsConfig.AllowCredentials();
    corsConfig.WithOrigins("http://127.0.0.1:3000", "https://127.0.0.1:3000", "https://avito.micmaclaynd.ru");
});

app.UseSwagger(swaggerConfig => {
    swaggerConfig.RouteTemplate = app.Configuration.GetOrThrow("Analytics:Swagger:RouteTemplate");
});
app.UseSwaggerUI(swaggerUiConfig => {
    swaggerUiConfig.RoutePrefix = app.Configuration.GetOrThrow("Analytics:Swagger:RoutePrefix");
});

app.MapControllers();

app.Run();
