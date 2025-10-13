using Cambio.Infra;
using Cambio.Infra.Repository;
using Cambio.Service;

var builder = WebApplication.CreateBuilder(args);

// Initialize database
DatabaseConfig.Initialize();

// Dependency Injection
builder.Services.AddSingleton<ExchangeHistoryRepository>();
builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddTransient<ExchangeRateService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cambio API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
