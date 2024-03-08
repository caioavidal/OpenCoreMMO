using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NeoServer.Shared.IoC.Modules;
using NeoServer.Web.API.HttpFilters;
using NeoServer.Web.API.IoC.Modules;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Extensions;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Integrations;

namespace NeoServer.Web.API;

public class Startup
{
    #region constructors

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;

        var builder = new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    #endregion

    #region properties

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }
    
    #endregion

    #region public methods implementations

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddBehaviours();
        services.AddServicesApi();
        services.AddAutoMapperProfiles();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardLimit = 2;
            options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "NeoServer.API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement());

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            c.OperationFilter<SwaggerJsonIgnoreFilter>();
        });

        services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.Newtonsoft);

        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                }
            );
        
        services.AddLogger(Configuration);
        services.AddDatabases(Configuration);
        services.AddRepositories();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    #endregion
}