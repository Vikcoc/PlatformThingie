using System.Reflection;
using PlatformInterfaces;


var assemblyFiles = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "*.dll");

//leaving without try catch for System.TypeLoadException so that mismatched dll versions are cought early
//should read about versioning and find a proper way to resolve https://learn.microsoft.com/en-us/dotnet/standard/assembly/versioning
var assemblies = assemblyFiles.Select(Assembly.LoadFrom)
    .Select(x => (Assembly: x, DefinitionType: x.ExportedTypes.FirstOrDefault(y => typeof(IPlatformComponentDefinition).IsAssignableFrom(y))))
    .Where(x => x.DefinitionType != null && !x.DefinitionType.IsAbstract)
    .Select(x => (x.Assembly, Definition: (IPlatformComponentDefinition)Activator.CreateInstance(x.DefinitionType!)!))
    .ToList();

foreach (var assembly in assemblies)
    Console.WriteLine(assembly.Definition.GivenName);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

foreach(var assembly in assemblies)
    assembly.Definition.AddServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

foreach(var assembly in assemblies)
    assembly.Definition.AddRoutes(app);

app.MapGet("/yes", () => "It worked");

app.Run();
