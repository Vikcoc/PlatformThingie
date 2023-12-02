using System.Reflection;
using PlatformInterfaces;


var assemblyFiles = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "*.dll");

var assemblies = assemblyFiles.Select(Assembly.LoadFrom)
    .Select(x => (Assembly: x, DefinitionType: x.ExportedTypes.FirstOrDefault(y => typeof(IPlatformComponentDefinition).IsAssignableFrom(y))))
    .Where(x => x.DefinitionType != null && !x.DefinitionType.IsAbstract)
    .ToList();

foreach (var assembly in assemblies)
{
    var def = (IPlatformComponentDefinition)Activator.CreateInstance(assembly.DefinitionType!)!;
    Console.WriteLine(def.GivenName);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
