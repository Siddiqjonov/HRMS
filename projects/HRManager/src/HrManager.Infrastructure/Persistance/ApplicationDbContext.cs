using HrManager.Application.Common.Interfaces;

namespace HrManager.Infrastructure.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<AbsenceRequest> AbsenceRequests { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<EmployeeAbsenceBalance> EmployeeAbsenceBalances { get; set; }

    public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Schedule> Schedules { get; set; }

    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Position>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Schedule>().HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("hr");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
