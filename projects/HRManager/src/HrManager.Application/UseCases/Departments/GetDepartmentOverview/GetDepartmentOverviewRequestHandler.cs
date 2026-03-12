using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Services;
using HrManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Departments.GetDepartmentOverview;

public class GetDepartmentOverviewRequestHandler(
    IApplicationDbContext context,
    IDateTimeService dateTimeService)
    : IRequestHandler<GetDepartmentOverviewRequest, DepartmentOverviewResponse>
{
    public async Task<DepartmentOverviewResponse> Handle(GetDepartmentOverviewRequest request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .Include(d => d.Employees)
            .ThenInclude(e => e.WorkSchedule)
            .FirstOrDefaultAsync(d => d.Id == request.departmentId, cancellationToken)
            ?? throw new NotFoundException($"Department with id {request.departmentId} not found");

        var today = DateOnly.FromDateTime(dateTimeService.UtcNow);
        var employeeIds = department.Employees.Select(e => e.Id).ToList();

        // Get today's attendance records for all employees in the department
        var todayAttendance = await context.AttendanceRecords
            .Where(a => employeeIds.Contains(a.EmployeeId) && a.Date == today)
            .Select(a => new
            {
                a.EmployeeId,
                a.CheckIn,
                a.CheckOut,
                a.IsLate,
            })
            .ToListAsync(cancellationToken);

        // Get approved absence requests that include today
        var approvedLeaves = await context.AbsenceRequests
            .Where(ar => employeeIds.Contains(ar.EmployeeId) &&
                         ar.RequestStatus == RequestStatus.Approved &&
                         ar.StartDate <= today &&
                         ar.EndDate >= today)
            .Select(ar => ar.EmployeeId)
            .ToListAsync(cancellationToken);

        // Get upcoming approved leaves (including today and future dates, up to 30 days ahead)
        var futureDate = today.AddDays(30);
        var upcomingLeaves = await context.AbsenceRequests
            .Include(ar => ar.Employee)
            .Where(ar => employeeIds.Contains(ar.EmployeeId) &&
                         ar.RequestStatus == RequestStatus.Approved &&
                         ar.EndDate >= today &&
                         ar.StartDate <= futureDate)
            .OrderBy(ar => ar.StartDate)
            .Select(ar => new LeaveCalendarItem(
                ar.Id,
                ar.Employee.FirstName + " " + ar.Employee.LastName,
                ar.RequestType.ToString(),
                ar.StartDate,
                ar.EndDate,
                ar.RequestStatus.ToString()))
            .ToListAsync(cancellationToken);

        var teamMembers = new List<TeamMemberAttendanceInfo>();
        int presentCount = 0;
        int absentCount = 0;
        int lateCount = 0;
        int onLeaveCount = 0;

        foreach (var employee in department.Employees)
        {
            string status;
            string? checkInTime = null;
            string? checkOutTime = null;

            // Check if employee is on leave
            if (approvedLeaves.Contains(employee.Id))
            {
                status = "on-leave";
                onLeaveCount++;
            }
            else
            {
                // Check attendance record
                var attendance = todayAttendance.FirstOrDefault(a => a.EmployeeId == employee.Id);

                if (attendance != null)
                {
                    // Employee has checked in - they are present
                    checkInTime = attendance.CheckIn.ToString("HH:mm");
                    checkOutTime = attendance.CheckOut?.ToString("HH:mm");
                    
                    // All checked-in employees count as present
                    presentCount++;

                    if (attendance.IsLate)
                    {
                        status = "late";
                        lateCount++; // Track separately how many present employees were late
                    }
                    else
                    {
                        status = "present";
                    }
                }
                else
                {
                    // Employee has not checked in and is not on leave
                    status = "absent";
                    absentCount++;
                }
            }

            teamMembers.Add(new TeamMemberAttendanceInfo(
                employee.Id,
                $"{employee.FirstName} {employee.LastName}",
                status,
                checkInTime,
                checkOutTime));
        }

        return new DepartmentOverviewResponse(
            department.Id,
            department.Name,
            department.Description,
            department.Employees.Count,
            presentCount,
            absentCount,
            lateCount,
            onLeaveCount,
            teamMembers,
            upcomingLeaves);
    }
}
