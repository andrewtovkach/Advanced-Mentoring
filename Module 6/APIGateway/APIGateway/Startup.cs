using System.Collections.Generic;
using System.Threading.Tasks;
using APIGateway.Extensions;
using APIGateway.Filters;
using APIGateway.Middlewares;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ApiRequestFilter));
            });

            services.AddOcelot(Configuration)
                .AddCacheManager(settings =>
                {
                    settings.WithDictionaryHandle();
                });

            services.AddJwtAuthentication(Configuration);
            services.AddDefaultCorrelationId();

            services.AddMvc();
            services.AddApplicationInsightsTelemetry();

            services.AddSwaggerForOcelot(Configuration,
            (o) =>
            {
                o.GenerateDocsForAggregates = true;
                o.GenerateDocsForGatewayItSelf = false;

                o.GenerateDocsDocsForGatewayItSelf(opt =>
                {
                    opt.DocumentFilter<SwaggerDocumentFilter>();
                    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.DefaultModelsExpandDepth(-1);
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseCorrelationId();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseMiddleware<CorrelationIdContextLogger>();
            
            await app.UseOcelot();
        }
    }
}
