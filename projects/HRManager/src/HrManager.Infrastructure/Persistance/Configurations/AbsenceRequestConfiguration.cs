namespace HrManager.Infrastructure.Persistance.Configurations;

public class AbsenceRequestConfiguration : IEntityTypeConfiguration<AbsenceRequest>
{
    public void Configure(EntityTypeBuilder<AbsenceRequest> builder)
    {

        builder.Property(ar => ar.RequestStatus)
            .HasConversion<string>();

        builder.Property(ar => ar.RequestType)
            .HasConversion<string>();
    }
}
