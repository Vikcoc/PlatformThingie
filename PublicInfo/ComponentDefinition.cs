﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace PublicInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Public info";

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            //general stuff
            endpoints.MapGet("/public/color",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "color.css"), "text/css"));
            endpoints.MapGet("/public/font",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "font.css"), "text/css"));
            endpoints.MapGet("/favicon.ico",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "modsig.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "style.css"), "text/css"));
            endpoints.MapGet("/public/topbar",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "topbar.css"), "text/css"));
            endpoints.MapGet("/public/material-components",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "material-components.js"), "text/javascript"));
            endpoints.MapGet("/public/back-logo",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "back-logo.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/plus-logo",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "plus-logo.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/save-logo",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "save-logo.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/trashcan-logo",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "trashcan-logo.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/topbar/signed-in",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "signed-in.js"), "text/javascript"));
            endpoints.MapGet("/public/authenticated-fetch",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "authenticated-fetch.js"), "text/javascript"));

            //the index i guess
            endpoints.MapGet("/", () => Results.Redirect("/about"));

            //the about page
            endpoints.MapGet("/about",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "about.html"), "text/html"));
            endpoints.MapGet("/about/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "about.css"), "text/css"));

            //the privacy page
            endpoints.MapGet("/privacy",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "privacy", "privacy.html"), "text/html"));

        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
