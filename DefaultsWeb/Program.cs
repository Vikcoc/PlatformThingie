using DefaultsAndStuff;
using PlatformInterfaces;
using TemplatingWeb;

Startup.BuildAndStart(args, new List<IPlatformComponentDefinition>
{
    new ComponentDefinition()
});