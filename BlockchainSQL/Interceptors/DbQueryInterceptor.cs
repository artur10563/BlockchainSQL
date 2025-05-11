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
    private readonly bool _simplifyQueryLogging;

    public DbQueryInterceptor(
        IBlockChainService blockchain,
        QueryValidatorHelper queryValidatorHelper,
        RequestContext requestContext,
        BlockchainOptions blockchainOptions)
    {
        _blockchain = blockchain;
        _queryValidatorHelper = queryValidatorHelper;
        _requestContext = requestContext;
        _simplifyQueryLogging = blockchainOptions.SimplifyQueryLogging;
    }

    #region Async

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        await TryLogIfMaliciousAsync(command);
        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await TryLogIfMaliciousAsync(command);
        return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        await TryLogIfMaliciousAsync(command);
        return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    #endregion

    #region Sync

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        TryLogIfMaliciousFireAndForget(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        TryLogIfMaliciousFireAndForget(command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        TryLogIfMaliciousFireAndForget(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    #endregion

    #region Core logic

    private bool IsSuspicious(DbCommand command)
    {
        return command.Parameters.Cast<DbParameter>()
            .Any(p => _queryValidatorHelper.IsSuspiciousParam(p.Value?.ToString()));
    }

    private string BuildLogJson(DbCommand command)
    {
        var log = new LogEntry()
        {
            Timestamp = DateTime.UtcNow.ToString("o"),
            Ip = _requestContext.IpAddress ?? "unknown",
            Query = _simplifyQueryLogging ? "" : command.CommandText,
            Endpoint = _requestContext.Endpoint ?? "unknown",
            Parameters = command.Parameters.Cast<DbParameter>().Select(p=>new Parameter()
            {
                DbType = p.DbType.ToString(),
                ParameterName = p.ParameterName,
                Value = p.Value?.ToString() ?? string.Empty
            }).ToList()
        };
        
        return JsonSerializer.Serialize(log);
    }

    private async Task TryLogIfMaliciousAsync(DbCommand command)
    {
        if (!IsSuspicious(command)) return;

        var json = BuildLogJson(command);
        await _blockchain.AddLogAsync(json);
    }

    private void TryLogIfMaliciousFireAndForget(DbCommand command)
    {
        if (!IsSuspicious(command)) return;

        var json = BuildLogJson(command);
        _ = Task.Run(async () =>
        {
            try
            {
                await _blockchain.AddLogAsync(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Blockchain log failed: {ex.Message}");
            }
        });
    }

    #endregion
}