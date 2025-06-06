using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.BusinessLogic;
using WebApi.DBEntityFramework;
using WebApi.HybridCacheService;
using WebApi.Interfaces;
using WebApi.Model;
using WebApi.Wrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});


// Add in-memory cache
builder.Services.AddMemoryCache();

// Add Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Redis default port
    options.InstanceName = "RedisHybrid_";
});




builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<ConfigSettings>(builder.Configuration.GetSection("Twitter"));
builder.Services.AddScoped<ILoginLogic, LoginLogic>();
builder.Services.AddScoped<ILoginWrapper, LoginWrapper>();
builder.Services.AddScoped<ITweetDashboardLogic, TweetDashboardLogic>();
builder.Services.AddScoped<ITweetDashboardWrapper, TweetDashboardWrapper>();
builder.Services.AddSingleton<IHybridCache, HybridCacheLogic>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();     // Required for Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddControllers(options => { options.ModelBinderProviders.Insert(0, new ComplexTypeModelBindingProvider()); }).AddNewtonsoftJson();
var app = builder.Build();
app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
