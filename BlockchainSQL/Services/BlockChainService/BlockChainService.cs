using System.Numerics;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace BlockchainSQL.Services.BlockChainService;

public class BlockChainService : IBlockChainService
{
    private readonly Web3 _web3;
    private readonly Contract _contract;

    private readonly BlockchainOptions _options;

    public BlockChainService(IOptions<BlockchainOptions> options)
    {
        _options = options.Value;

        _web3 = new Web3(_options.RpcUrl);
        _contract = _web3.Eth.GetContract(_options.Abi, _options.ContractAddress);
    }

    private Web3 GetSigningWeb3() => new Web3(new Nethereum.Web3.Accounts.Account(_options.PrivateKey), _options.RpcUrl);

    public async Task<string> GetLatestLogAsync()
    {
        var function = _contract.GetFunction(_options.Functions.GetLatestLog);
        return await function.CallAsync<string>();
    }

    public async Task<string> GetLogAsync(BigInteger index)
    {
        var function = _contract.GetFunction(_options.Functions.GetLog);
        return await function.CallAsync<string>(index);
    }

    public async Task<BigInteger> GetCountAsync()
    {
        var function = _contract.GetFunction(_options.Functions.GetCount);
        return await function.CallAsync<BigInteger>();
    }

    public async Task<string> AddLogAsync(string logJson)
    {
        var account = new Nethereum.Web3.Accounts.Account(_options.PrivateKey);
        var web3 = new Web3(account, _options.RpcUrl);
        var function = web3.Eth.GetContract(_options.Abi, _options.ContractAddress).GetFunction(_options.Functions.AddLog);
        var gas = await function.EstimateGasAsync(_options.AccountAddress, null, null, logJson);
        var txHash = await function.SendTransactionAsync(_options.AccountAddress, gas, null, null, logJson);
        return txHash;
    }
}