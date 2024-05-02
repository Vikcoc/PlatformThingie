using TemplatingWeb;

Startup.BuildAndStart(args,
[
    new PublicInfo.ComponentDefinition(),
    new AuthFrontend.ComponentDefinition(),
    new UserInfo.ComponentDefinition(),
    new InventoryInfo.ComponentDefinition(),
    new InventoryScripts.ComponentDefinition(),
    new InvTemplateInfo.ComponentDefinition(),
    new UserPermissionConsumer.ComponentDefinition(),
    new InventoryTemplateConsumer.ComponentDefinition(),
]);