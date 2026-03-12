using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AbsenceRequest> AbsenceRequests { get; set; }

    DbSet<Department> Departments { get; set; }

    DbSet<Employee> Employees { get; set; }

    DbSet<EmployeeAbsenceBalance> EmployeeAbsenceBalances { get; set; }

    DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

    DbSet<Position> Positions { get; set; }

    DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    DbSet<Schedule> Schedules { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
