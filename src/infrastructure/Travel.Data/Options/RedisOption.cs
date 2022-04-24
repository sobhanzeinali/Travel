namespace Travel.Data.Options;

public class RedisOption
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Password { get; set; }
    public bool UsePassword { get; set; }
    public string ConnectionString => UsePassword ? $"{Host}:{Port},password={Password}" : $"{Host}:{Port}";
}