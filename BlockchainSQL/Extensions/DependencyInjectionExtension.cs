using BlockchainSQL.Helpers;
using BlockchainSQL.Interceptors;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddBlockchainSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RequestContext>();
        services.Configure<BlockchainOptions>(configuration.GetSection("Blockchain"));
        services.AddScoped<IBlockChainService, BlockChainService>();
        services.AddScoped<QueryValidatorHelper>();
        services.AddScoped<DbQueryInterceptor>();
        return services;
    }
}