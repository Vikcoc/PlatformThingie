﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace InventoryScripts
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Inventory";

        public void AddRoutes(IEndpointRouteBuilder endpoints, IConfiguration config)
        {
            endpoints.MapGet(config["ScriptsRoute"] + "someaction",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "someaction.js"), "text/javascript"));
            endpoints.MapGet(config["ScriptsRoute"] + "someaction2",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "someaction2.js"), "text/javascript"));
            endpoints.MapGet(config["ScriptsRoute"] + "displayheader",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "displayheader.js"), "text/javascript"));
            endpoints.MapGet(config["ScriptsRoute"] + "name_and_picture",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "name_and_picture.js"), "text/javascript"));
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
