using PlatformInterfaces;
using TemplatingWeb;

Startup.BuildAndStart(args, new List<IPlatformComponentDefinition>
{
    new DefaultsAndStuff.ComponentDefinition(),
    new AuthFrontend.ComponentDefinition()
});