import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { GetEmployeeByEmailRequest } from '../../../core/models/request-models/employee-request.model';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../core/services/dashboard-refresh.service';
import { UserRole } from '../../../core/models/roles.model';

// Import all widget components
import { WelcomeWidgetComponent } from '../widgets/welcome-widget/welcome-widget.component';
import { AttendanceClockWidgetComponent } from '../widgets/attendance-clock-widget/attendance-clock-widget.component';
import { AttendanceSummaryWidgetComponent } from '../widgets/attendance-summary-widget/attendance-summary-widget.component';
import { UpcomingLeavesWidgetComponent } from '../widgets/upcoming-leaves-widget/upcoming-leaves-widget.component';
import { RecentRequestsWidgetComponent } from '../widgets/recent-requests-widget/recent-requests-widget.component';
import { PendingApprovalsWidgetComponent } from '../widgets/pending-approvals-widget/pending-approvals-widget.component';
import { TeamAttendanceWidgetComponent } from '../widgets/team-attendance-widget/team-attendance-widget.component';
import { DepartmentOverviewWidgetComponent } from '../widgets/department-overview-widget/department-overview-widget.component';
import { TeamLeaveCalendarWidgetComponent } from '../widgets/team-leave-calendar-widget/team-leave-calendar-widget.component';
import { EmployeeStatisticsWidgetComponent } from '../widgets/employee-statistics-widget/employee-statistics-widget.component';
import { CompanyAttendanceWidgetComponent } from '../widgets/company-attendance-widget/company-attendance-widget.component';
import { UpcomingBirthdaysWidgetComponent } from '../widgets/upcoming-birthdays-widget/upcoming-birthdays-widget.component';

@Component({
  selector: 'app-dashboard-main',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    WelcomeWidgetComponent,
    AttendanceClockWidgetComponent,
    AttendanceSummaryWidgetComponent,
    UpcomingLeavesWidgetComponent,
    RecentRequestsWidgetComponent,
    PendingApprovalsWidgetComponent,
    TeamAttendanceWidgetComponent,
    DepartmentOverviewWidgetComponent,
    TeamLeaveCalendarWidgetComponent,
    EmployeeStatisticsWidgetComponent,
    CompanyAttendanceWidgetComponent,
    UpcomingBirthdaysWidgetComponent
  ],
  templateUrl: './dashboard-main.component.html',
  styleUrl: './dashboard-main.component.css'
})
export class DashboardMainComponent implements OnInit, OnDestroy {
  private authService = inject(AuthenticationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);

  employeeId: string | null = null;
  userName: string = '';
  isManagerOrAdmin: boolean = false;
  isDepartmentManager: boolean = false;
  departmentId: string | null = null;
  
  // Auto-refresh interval (5 minutes = 300000ms)
  private autoRefreshInterval: any;
  private readonly REFRESH_INTERVAL = 300000;

  departmentStats = {
    name: 'Engineering',
    totalEmployees: 45,
    presentToday: 38,
    onLeave: 5,
    averageAttendance: 87
  };

  ngOnInit(): void {
    this.checkUserRole();
    this.loadCurrentUserEmployeeId();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  private startAutoRefresh(): void {
    this.autoRefreshInterval = setInterval(() => {
      this.refreshDashboard();
    }, this.REFRESH_INTERVAL);
  }

  private stopAutoRefresh(): void {
    if (this.autoRefreshInterval) {
      clearInterval(this.autoRefreshInterval);
    }
  }

  checkUserRole(): void {
    // Check if user has HrManager or Admin role
    this.isManagerOrAdmin = this.authService.hasRole(UserRole.HrManager) || 
                            this.authService.hasRole(UserRole.Admin);
  }

  loadCurrentUserEmployeeId(): void {
    const userEmail = this.authService.userEmail;
    if (!userEmail) {
      this.notificationService.warning('Unable to get user email. Please try logging in again.');
      return;
    }

    const request: GetEmployeeByEmailRequest = { email: userEmail };
    this.apiService.getEmployeeByEmail(request).subscribe({
      next: (employee) => {
        this.employeeId = employee.id;
        this.userName = `${employee.firstName} ${employee.lastName}`;
        this.isDepartmentManager = employee.isManagerOfDepartment;
        this.departmentId = employee.departmentId;
        this.loadDashboardData();
      },
      error: (error) => {
        this.notificationService.handleError(error);
      }
    });
  }

  loadDashboardData(): void {
    // Dashboard data is now loaded by individual widgets
    // This method can be used for any centralized data loading if needed
  }

  onAttendanceUpdated(): void {
    // Refresh attendance-related widgets
    this.refreshService.refreshAttendanceWidgets();
  }

  onApprovalProcessed(): void {
    // Refresh leave-related widgets after approval/rejection
    // This already includes team-leave-calendar
    this.refreshService.refreshLeaveWidgets();
  }

  private refreshDashboard(): void {
    this.refreshService.refreshAll();
  }
}
