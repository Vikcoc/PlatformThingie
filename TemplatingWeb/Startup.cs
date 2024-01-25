﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformInterfaces;

namespace TemplatingWeb
{
    public static class Startup
    {
        public static void BuildAndStart(string[] args, IEnumerable<IPlatformComponentDefinition> components)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            foreach (var component in components)
                component.AddServices(builder.Services);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            foreach (var component in components)
                component.AddRoutes(app);

            app.Run();
        }
    }
}