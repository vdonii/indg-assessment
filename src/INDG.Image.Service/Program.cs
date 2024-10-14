using INDG.Image.Service;
using INDG.Image.Service.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

configurationBuilder.AddSystemsManager("/indg/service", true);

configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true);
var configuration = configurationBuilder.Build();

IocBindings.Configure(builder.Services, configuration);

builder.Services.AddAutoMapper(typeof(ImageMappingProfile));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
