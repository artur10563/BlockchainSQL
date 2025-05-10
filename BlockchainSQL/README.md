# BlockchainSQL - Proof of Concept for SQL Injection Detection and Blockchain Logging
This project is a **proof of concept** demonstrating a library that detects potentially **SQL-injected queries** in a database and logs them to a **blockchain** for further analysis and tracking.
## Features
- **SQL Injection Detection**: The library intercepts SQL queries executed against a database and checks for patterns indicative of SQL injection attempts.
- **Blockchain Logging**: When a suspicious SQL query is detected, the query is logged to a blockchain via a smart contract. This provides a tamper-proof record of suspicious activities.
## Installation
You can install the library from **NuGet**:
```bash
dotnet add package BlockchainSQL
```

## Configuration

To use this library, you need to configure the connection to the blockchain and the smart contract that handles the logging of detected queries. The configuration is done via **appsettings.json** (or another configuration source, depending on your setup).

### Example `appsettings.json` Configuration:

```json
{
  "Blockchain": {
    "RpcUrl": "https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID",  // Ethereum RPC URL
    "ContractAddress": "0xYourContractAddress",                      // Your smart contract address
    "Abi": "[YOUR_CONTRACT_ABI_JSON]",                                // The ABI (Application Binary Interface) of your smart contract
    "PrivateKey": "0xYourPrivateKey",                                 // Your Ethereum account private key
    "AccountAddress": "0xYourAccountAddress",                         // Your Ethereum account address for signing transactions
    "Functions": {
      "AddLog": "addLog",                                             // Function name for adding logs to the blockchain
      "GetLatestLog": "getLatestLog",                                  // Function name for fetching the latest log
      "GetLog": "getLog",                                              // Function name for fetching a log by index
      "GetCount": "getCount",                                           // Function name for fetching the total count of logs
      "GetAllLogs": "getAllLogs"                                        //Function name for fetching all logs. EXPENSIVE GAS
    }
  }
}
```
### In order to log more info, need to register the middleware
```bash
app.UseMiddleware<RequestContextMiddleware>();
```