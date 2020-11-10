using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionTier.Startup))]
namespace AzureFunctionTier
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) =>
            builder.Services.AddCosmosRepository();
    }
}
