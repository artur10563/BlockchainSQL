using BlockchainSQL.Helpers;
using BlockchainSQL.Interceptors;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddBlockchainSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RequestContext>();
        services.Configure<BlockchainOptions>(configuration.GetSection("Blockchain"));
        services.AddScoped<IBlockChainService, BlockChainService>();
        return services;
    }

    public static IServiceCollection AddQueryInterceptor(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<QueryValidatorHelper>();
        services.AddScoped<DbQueryInterceptor>();
        return services;
    }
    public static IApplicationBuilder UseRequestInfoMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestContextMiddleware>();
    }
}