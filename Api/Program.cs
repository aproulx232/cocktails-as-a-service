using Azure.Identity;

namespace Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, configBuilder) =>
                        {
                            //var builtConfig = configBuilder.Build();
                            //var vaultUri = new Uri($"https://{builtConfig.GetValue<string>("CocktailsAsAServiceVaultName")}.vault.azure.net/");
                            //configBuilder.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
                        })
                        .UseStartup<Startup>();
                });
    }
}