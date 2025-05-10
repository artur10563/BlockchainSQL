using BlockchainSQL.Helpers;
using BlockchainSQL.Interceptors;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainSQL.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddBlockchainSql(this IServiceCollection services)
    {
        services.AddScoped<IBlockChainService, BlockChainService>();
        services.AddScoped<QueryValidatorHelper>();
        services.AddScoped<DbQueryInterceptor>();

        return services;
    }
}