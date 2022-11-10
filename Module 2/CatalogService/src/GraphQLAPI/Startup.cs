using CatalogService.Application;
using CatalogService.Application.Common.Correlation;
using CatalogService.GraphQLAPI.Extensions;
using CatalogService.GraphQLAPI.GraphQL.GraphQLSchema;
using CatalogService.Infrastructure;
using CatalogService.MessageBroker.Producer;
using Confluent.Kafka;
using CorrelationId;
using CorrelationId.DependencyInjection;
using GraphiQl;
using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CatalogService.GraphQLAPI
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
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services.AddTransient<ICorrelationIdInitializer, CorrelationIdInitializer>();
            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews();

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            services.AddScoped<AppSchema>();

            services.AddGraphQL()
                .AddSystemTextJson()
                .AddGraphTypes(typeof(AppSchema), ServiceLifetime.Scoped)
                .AddDataLoader();

            services.AddControllers();

            var producerConfig = new ProducerConfig(new ClientConfig
            {
                BootstrapServers = Configuration["Kafka:ClientConfigs:BootstrapServers"]
            });

            services.AddJwtAuthentication(Configuration);
            services.AddDefaultCorrelationId();

            services.AddSingleton(producerConfig);
            services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseCorrelationId();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGraphQL<AppSchema>();
            app.UseGraphiQl("/graphql");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
