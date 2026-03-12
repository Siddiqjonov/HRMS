namespace HrManager.Infrastructure.Persistance.Configurations.Settings;

public class FirebaseStorageSettings(string bucketName, string serviceAccountKeyPath, string? serviceAccountKeyJson = null)
{
    public string BucketName { get; private set; } = bucketName;

    public string ServiceAccountKeyPath { get; private set; } = serviceAccountKeyPath;

    public string? ServiceAccountKeyJson { get; private set; } = serviceAccountKeyJson;
}
