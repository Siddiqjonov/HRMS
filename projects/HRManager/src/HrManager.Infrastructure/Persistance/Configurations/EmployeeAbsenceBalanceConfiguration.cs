namespace HrManager.Infrastructure.Persistance.Configurations;

public class EmployeeAbsenceBalanceConfiguration : IEntityTypeConfiguration<EmployeeAbsenceBalance>
{
    public void Configure(EntityTypeBuilder<EmployeeAbsenceBalance> builder)
    {
        builder.Property(e => e.AbsenceType)
            .HasConversion<string>();
    }
}
