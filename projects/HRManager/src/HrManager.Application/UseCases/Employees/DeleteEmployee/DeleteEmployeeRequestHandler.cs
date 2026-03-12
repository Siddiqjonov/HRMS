using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.DeleteEmployee
{
    public class DeleteEmployeeRequestHandler(
     IApplicationDbContext context,
     ICurrentUserService currentUserService,
     IDateTimeService dateTimeService
 ) : IRequestHandler<DeleteEmployeeRequest>
    {
        public async Task Handle(DeleteEmployeeRequest request, CancellationToken cancellationToken)
        {

            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee is null)
            {
                throw new NotFoundException($"Employee with ID {request.Id} not found.");
            }

            employee.IsDeleted = true;
            employee.DeletedBy = currentUserService.UserId;
            employee.DeletedOnUtc = dateTimeService.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
