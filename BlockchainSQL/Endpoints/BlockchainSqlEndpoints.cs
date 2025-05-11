using System.Numerics;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace BlockchainSQL.Web.Endpoints;

public static class BlockchainSqlEndpoints
{
    public static IEndpointRouteBuilder RegisterBlockchainSqlEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("blockchainsql/logs");

        group.MapGet("/latest", async (IBlockChainService _blockchain) =>
        {
            var dlog = await _blockchain.GetLatestLogAsync();
            var log = JsonSerializer.Deserialize<LogEntry>(dlog);
            return Results.Ok(log);
        });

        group.MapGet("", async (IBlockChainService _blockchain) =>
        {
            var dlogs = await _blockchain.GetAllLogsAsync();
            var logs = dlogs.Select(log => JsonSerializer.Deserialize<LogEntry>(log)).ToList();
            return Results.Ok(logs);
        });

        group.MapGet("/{index}", async (IBlockChainService _blockchain, BigInteger index) =>
        {
            var dlog = await _blockchain.GetLogAsync(index);
            var log = JsonSerializer.Deserialize<LogEntry>(dlog);
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