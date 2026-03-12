namespace HrManager.Infrastructure.Persistance.Configurations.Settings;

public class AzureBlobSettings(string connectionString, string containerName)
{
    public string ConnectionString { get; private set; } = connectionString;

    public string ContainerName { get; private set; } = containerName;
}
