using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CartingService.API;
using CartingService.API.Configuration;
using CartingService.API.Extensions;
using CartingService.Domain.Repositories;
using CartingService.MessageBroker.Consumer;
using CartingService.Persistence.Repositories;
using CartingService.Presentation.Middlewares;
using CartingService.Services;
using CartingService.Services.EventHandlers;
using CartingService.Services.Events;
using CatringService.Services.Abstractions;
using Confluent.Kafka;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CartingService.Presentation
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
            services.AddControllers();
            services.AddApplicationInsightsTelemetry();

            services.AddApiVersioning(opt => {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(opt => opt.GroupNameFormat = "'v'VVV");

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IUserCartService, UserCartService>();
            services.AddTransient<IRepositoryManager>(provider => new RepositoryManager(connectionString));
            services.AddTransient<ExceptionHandlingMiddleware>();

            services.AddJwtAuthentication(Configuration);
            services.AddDefaultCorrelationId();

            var clientConfig = new ClientConfig
            {
                BootstrapServers = Configuration["Kafka:ClientConfigs:BootstrapServers"]
            };

            var consumerConfig = new ConsumerConfig(clientConfig)
            {
                GroupId = "SourceApp",
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000
            };

            services.AddSingleton(consumerConfig);

            services.AddScoped<IKafkaHandler<string, ItemChanged>, ItemChangedHandler>();
            services.AddSingleton(typeof(IKafkaConsumer<,>), typeof(KafkaConsumer<,>));
            services.AddHostedService<ItemChangedConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(
                opt => {
                    foreach (var description in provider.ApiVersionDescriptions.Select(description => description.GroupName))
                    {
                        opt.SwaggerEndpoint(
                            $"/swagger/{description}/swagger.json",
                            description.ToUpperInvariant());
                    }
                });
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCorrelationId();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseMiddleware<CorrelationIdContextLogger>();
        }
    }
}
