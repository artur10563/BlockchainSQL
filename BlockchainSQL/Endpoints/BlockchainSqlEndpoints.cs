using System.Numerics;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BlockchainSQL.Web.Endpoints;

public static class BlockchainSqlEndpoints
{
    public static IEndpointRouteBuilder RegisterBlockchainSqlEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("blockchainsql/logs");

        group.MapGet("/latest", async (IBlockChainService _blockchain) =>
        {
            var log = await _blockchain.GetLatestLogAsync();
            return Results.Ok(log);
        });
        
        group.MapGet("", async (IBlockChainService _blockchain) =>
        {
            var logs = await _blockchain.GetAllLogsAsync();
            return Results.Ok(logs);
        });
        
        group.MapGet("/{index}", async (IBlockChainService _blockchain, BigInteger index) =>
        {
            var log = await _blockchain.GetLogAsync(index);
            return Results.Ok(log);
        });
        
        group.MapGet("/count", async (IBlockChainService _blockchain) =>
        {
            var count = await _blockchain.GetCountAsync();
            return Results.Ok(count);
        });
        
        

        return app;
    }
}