using System.Numerics;

namespace BlockchainSQL.Services.BlockChainService;

public interface IBlockChainService
{
    Task<string> GetLatestLogAsync();
    Task<string> GetLogAsync(BigInteger index);
    Task<BigInteger> GetCountAsync();
    Task<string> AddLogAsync(string logJson);
}

public class BlockchainOptions
{
    public string Abi { get; set; } = "";
    public string RpcUrl { get; set; } = "";
    public string PrivateKey { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    public string AccountAddress { get; set; } = "";
    public BlockchainFunctionNames Functions { get; set; } = new();
}

public class BlockchainFunctionNames
{
    public string AddLog { get; set; } = "addLog";
    public string GetLatestLog { get; set; } = "getLatestLog";
    public string GetLog { get; set; } = "getLog";
    public string GetCount { get; set; } = "getCount";
}