namespace HrManager.Infrastructure.Persistance.Configurations;

public class EmployeeDocumentConfiguration : IEntityTypeConfiguration<EmployeeDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FilePath)
            .IsRequired().HasMaxLength(500);

        builder.Property(x => x.BlobName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.BlobUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ContainerName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.FileSizeInBytes)
            .IsRequired();

        builder.Property(x => x.ContentType)
           .HasMaxLength(150);

        builder.Property(x => x.UploadedByUserId)
            .IsRequired();

        builder.Property(x => x.UploadedAt)
            .IsRequired();
    }
}
