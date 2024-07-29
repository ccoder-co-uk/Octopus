using Octopus.Data;
using RazorLight;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var builder = WebApplication
    .CreateBuilder(args);

var engine = new RazorLightEngineBuilder()
    .UseFileSystemProject(Directory.GetCurrentDirectory() + "/Pages")
    .Build();

builder.Services.AddSingleton<IRazorLightEngine>(engine);
builder.Services.AddScoped<OctopusDbContext>(ctx => new(config.GetConnectionString("Energy")));

var app = builder.Build();

app.MapGet("", async (HttpContext context, IRazorLightEngine engine) =>
{
    var result = await engine.CompileRenderAsync<object>("Index", null);
    await context.Response.WriteAsync(result);
});

app.MapGet("Api/Data", async (HttpContext context, OctopusDbContext db) =>
    Results.Ok(db.DailyStats));

await app.RunAsync();