using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.ML;
using Sentiment.Logging;
using Sentiment.Model;

namespace Sentiment
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sentiment", Version = "v1" });
            });

            services.AddSignalR();

            services.AddLogging();

            services.AddScoped<IPredictionService, PredictionService>();

            services.AddPredictionEnginePool<SentimentData, SentimentPrediction>()
                .FromFile(modelName: "SentimentAnalysisModel", filePath:"..\\model\\MLModel.zip", watchForChanges: true);

           // Use this code to poll for changes from an external uri

           // services.AddPredictionEnginePool<SentimentData, SentimentPrediction>()
           //     .FromUri(
           //         modelName: "SentimentAnalysisModel",
           //         uri: "http://localhost:5000/MLModel.zip",
           //         period: TimeSpan.FromHours(1));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sentiment v1"));
            }

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LoggingHub>("/socket-logging");
            });
        }
    }
}
