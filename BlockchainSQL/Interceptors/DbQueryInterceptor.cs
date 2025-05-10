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

    public DbQueryInterceptor(
        IBlockChainService blockchain,
        QueryValidatorHelper queryValidatorHelper)
    {
        _blockchain = blockchain;
        _queryValidatorHelper = queryValidatorHelper;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ValidateDbCall(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        ValidateDbCall(command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        ValidateDbCall(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    private void ValidateDbCall(DbCommand command, bool simplifyQueryLogging = false)
    {
        var hasMaliciousParam = command.Parameters.Cast<DbParameter>()
            .Any(p => _queryValidatorHelper.IsSuspiciousParam(p.Value?.ToString()));

        if (!hasMaliciousParam) return;

        var queryInfo = new
        {
            query = command.CommandText,
            parameters =
                command.Parameters.Cast<DbParameter>().Select(p => new
                {
                    p.DbType,
                    p.ParameterName,
                    p.Value,
                }).ToList()
        };

        var jsonString = JsonSerializer.Serialize(queryInfo);

        _blockchain.AddLogAsync(jsonString);
    }
}