using TemplatingWeb;

Startup.BuildAndStart(args,
[
    new PublicInfo.ComponentDefinition(),
    new AuthFrontend.ComponentDefinition()
]);