using System.Numerics;

namespace BlockchainSQL.Services.BlockChainService;

public interface IBlockChainService
{
    Task<string> GetLatestLogAsync();
    Task<string> GetLogAsync(BigInteger index);
    Task<BigInteger> GetCountAsync();
    Task<string> AddLogAsync(string logJson);
    Task<List<string>> GetAllLogsAsync();
}

public class BlockchainOptions
{
    public string Abi { get; set; } = "";
    public string RpcUrl { get; set; } = "";
    public string PrivateKey { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    public string AccountAddress { get; set; } = "";
    public bool SimplifyQueryLogging { get; set; } = false;
    public BlockchainFunctionNames Functions { get; set; } = new();
}

public class BlockchainFunctionNames
{
    public string AddLog { get; set; } = "addLog";
    public string GetLatestLog { get; set; } = "getLatestLog";
    public string GetLog { get; set; } = "getLog";
    public string GetCount { get; set; } = "getCount";
    public string GetAllLogs { get; set; } = "getAllLogs";
}

public class LogEntry
{
    public string Timestamp { get; set; }
    public string Ip { get; set; }
    public string Query { get; set; }
    public string Endpoint { get; set; }
    public List<Parameter> Parameters { get; set; }
}

public class Parameter
{
    public string DbType { get; set; }
    public string ParameterName { get; set; }
    public string Value { get; set; }
}