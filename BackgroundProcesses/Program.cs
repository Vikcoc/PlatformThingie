using TemplatingWeb;

Startup.BuildAndStart(args,
[
    new UserPermissionConsumer.ComponentDefinition(),
    new InventoryTemplateConsumer.ComponentDefinition(),
]);