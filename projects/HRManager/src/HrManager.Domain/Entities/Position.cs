namespace HrManager.Domain.Entities;

public class Position : SoftDeletableAuditableEntity
{
    public Position() { }
  
    public Position(string title, Guid departmentId, long salaryMin, long salaryMax)
    {
        Title = title;
        DepartmentId = departmentId;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
    }

    public string Title { get; private set; }
        
    public Guid DepartmentId { get; set; }

    public Department Department { get; private set; }

    public long SalaryMin { get; private set; }

    public long SalaryMax { get; private set; }

    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
}
