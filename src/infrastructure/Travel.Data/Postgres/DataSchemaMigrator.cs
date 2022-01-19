using DbUp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Travel.Data.Options;

namespace Travel.Data.Postgres;

public class DataSchemaMigrator:IDataSchemaMigrator
{
    private PostgresOptions _options { get; }
    private ILogger<DataSchemaMigrator> _logger { get; }

    public DataSchemaMigrator(IOptions<PostgresOptions> options, ILogger<DataSchemaMigrator> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public void UpdateSchema()
    {
        _logger.LogWarning($"connecting to {_options.ConnectionStringWithoutSensitiveData}");
        
        EnsureDatabase.For.PostgresqlDatabase(_options.ConnectionString);

        DeployChanges
            .To
            .PostgresqlDatabase(_options.ConnectionString)
            .LogScriptOutput()
            .LogToConsole()
            .WithScriptsEmbeddedInAssembly(typeof(DataSchemaMigrator).Assembly, x => x.EndsWith("sql"))
            .WithTransactionPerScript()
            .Build()
            .PerformUpgrade();
    }
}