﻿using AuthFrontend.functionalities.loggingIn;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace AuthFrontend
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Login with Google";

        public void AddRoutes(IEndpointRouteBuilder endpoints, IConfiguration config)
        {
            LogInController.AddRoutes(endpoints);

            endpoints.MapGet("/login/test", () => "It worked")
                .RequireAuthorization(p => p.RequireClaim("purpose", "access"));

            endpoints.MapGet("/login",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "login", "login.html"), "text/html"));
            
            endpoints.MapGet("/login/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "login", "login.js"), "text/javascript"));
        }

        public void AddServices(IServiceCollection services)
        {
            LogInController.AddServices(services);
        }
    }
}
