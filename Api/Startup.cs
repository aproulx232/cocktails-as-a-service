using System.Net;
using Application;
using Infrastructure.CocktailDbService;
using Infrastructure.TableService;
using Microsoft.Extensions.Azure;
using Polly;
using Polly.Extensions.Http;

namespace Api
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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton<ICocktailProvider, CocktailProvider>();
            services.AddSingleton<IUserProvider, UserProvider>();
            services.AddSingleton<IViewProvider, ViewProvider>();

            services.AddHttpClient<ICocktailDbService, CocktailDbService>(client =>
            {
                client.BaseAddress = new Uri("https://www.thecocktaildb.com");
            }).AddPolicyHandler(GetRetryPolicy());

            services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddTableServiceClient(Configuration.GetConnectionString("TableStorage"));
            });
            services.AddSingleton<ITableServiceConfiguration>(new TableServiceConfiguration("Users"));
            services.AddSingleton<ITableService, TableService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadGateway)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
