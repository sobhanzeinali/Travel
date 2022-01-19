namespace Travel.Data.Options;

public class PostgresOptions
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DatabaseName { get; set; }
    public int Port { get; set; }

    public string ConnectionString =>
        $"Server={Host};Port={Port};Database={DatabaseName};User Id={Username};Password={Password};";

    public string ConnectionStringWithoutSensitiveData =>
        $"Server={Host};Port={Port};Database={DatabaseName};User Id={Username};Password=***;";
}