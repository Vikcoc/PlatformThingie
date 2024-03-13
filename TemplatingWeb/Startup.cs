using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PlatformInterfaces;

namespace TemplatingWeb
{
    public static class Startup
    {
        public static void BuildAndStart(string[] args, IEnumerable<IPlatformComponentDefinition> components)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication().AddJwtBearer(opt =>
            {
                var key = builder.Configuration.GetSection("SigningCredentials:Auth:JWK").Get<JsonWebKey>();
                var keySet = new JsonWebKeySet();
                keySet.Keys.Add(key);

                var parameters = new TokenValidationParameters()
                {
                    ValidIssuer = builder.Configuration["SigningCredentials:Auth:Issuer"],
                    ValidAudience = builder.Configuration["SigningCredentials:Auth:Audience"],
                    IssuerSigningKeys = keySet.Keys,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };

                opt.TokenValidationParameters = parameters;
            });
            builder.Services.AddAuthorization();

            foreach (var component in components)
                component.AddServices(builder.Services);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.Use(async (context, next) =>
                {
                    Console.WriteLine(context.Request.Path);
                    await next.Invoke();
                });

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            foreach (var component in components)
                component.AddRoutes(app);

            app.Run();
        }
    }
}
