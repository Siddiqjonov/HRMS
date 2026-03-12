namespace HrManager.Infrastructure.Persistance.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasOne(e => e.Department)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.DepartmentId);

        builder.Property(e => e.Gender)
            .HasConversion<string>();

        builder.OwnsOne(e => e.Address);

        builder.HasIndex(e => e.Email).IsUnique();
    }
}
