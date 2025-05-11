using System.Data.Common;
using System.Text.Json;
using BlockchainSQL.Helpers;
using BlockchainSQL.Services.BlockChainService;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlockchainSQL.Interceptors;

public class DbQueryInterceptor : DbCommandInterceptor
{
    private readonly IBlockChainService _blockchain;
    private readonly QueryValidatorHelper _queryValidatorHelper;
    private readonly RequestContext _requestContext;

    public DbQueryInterceptor(
        IBlockChainService blockchain,
        QueryValidatorHelper queryValidatorHelper, RequestContext requestContext)
    {
        _blockchain = blockchain;
        _queryValidatorHelper = queryValidatorHelper;
        _requestContext = requestContext;
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await ValidateDbCallAsync(command);
        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await ValidateDbCallAsync(command);
        return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await ValidateDbCallAsync(command);
        return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }
    
    private async Task ValidateDbCallAsync(DbCommand command, bool simplifyQueryLogging = false)
    {
        var hasMaliciousParam = command.Parameters.Cast<DbParameter>()
            .Any(p => _queryValidatorHelper.IsSuspiciousParam(p.Value?.ToString()));

        if (!hasMaliciousParam) return;

        var queryInfo = new
        {
            timestamp = DateTime.UtcNow.ToString("o"),
            ip = _requestContext.IpAddress ?? "unknown",
            query = simplifyQueryLogging
                ? _requestContext.Endpoint
                : command.CommandText,
            parameters =
                command.Parameters.Cast<DbParameter>().Select(p => new
                {
                    p.DbType,
                    p.ParameterName,
                    p.Value,
                }).ToList()
        };

        var jsonString = JsonSerializer.Serialize(queryInfo);

        await _blockchain.AddLogAsync(jsonString);
    }
}