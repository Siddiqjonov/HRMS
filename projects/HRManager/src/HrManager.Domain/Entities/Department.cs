namespace HrManager.Domain.Entities;

public class Department : SoftDeletableAuditableEntity
{
    public Department() { }
  
    public Department(string name, string description, Guid? managerId)
    {
        Name = name;
        Description = description;
        ManagerId = managerId;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public Guid? ManagerId { get; set; } 

    public Employee Manager { get; private set; }

    public ICollection<Position> Positions { get; private set; } = new List<Position>();

    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
}
