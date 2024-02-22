using PlatformInterfaces;
using TemplatingWeb;

Startup.BuildAndStart(args, new List<IPlatformComponentDefinition>
{
    new AuthFrontend.ComponentDefinition()
});