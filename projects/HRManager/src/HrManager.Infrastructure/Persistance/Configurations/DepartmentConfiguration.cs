namespace HrManager.Infrastructure.Persistance.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasOne(d => d.Manager)
               .WithOne()
               .HasForeignKey<Department>(d => d.ManagerId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasMany(d => d.Employees)
               .WithOne(e => e.Department)
               .HasForeignKey(e => e.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict); 
    }
}
