using TemplatingWeb;

Startup.BuildAndStart(args,
[
    new InventoryInfo.ComponentDefinition(),
    new InventoryScripts.ComponentDefinition(),
]);