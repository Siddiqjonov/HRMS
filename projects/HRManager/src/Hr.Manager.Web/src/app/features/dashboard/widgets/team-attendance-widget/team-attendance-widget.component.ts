import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Subscription, forkJoin } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { TeamMemberAttendanceInfo } from '../../../../core/models/response-models/department-overview-response.model';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

@Component({
  selector: 'app-team-attendance-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './team-attendance-widget.component.html',
  styleUrl: './team-attendance-widget.component.css'
})
export class TeamAttendanceWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private authService = inject(AuthenticationService);
  private refreshSubscription?: Subscription;
  
  private currentUserId: string | null = null;

  teamMembers: TeamMemberAttendanceInfo[] = [];
  isLoading = false;
  presentCount = 0;
  absentCount = 0;
  lateCount = 0;
  onLeaveCount = 0;
  departmentName = '';

  ngOnInit(): void {
    this.loadCurrentUserAndTeamAttendance();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'team-attendance') {
        this.loadCurrentUserAndTeamAttendance();
      }
    });
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadCurrentUserAndTeamAttendance(): void {
    const userEmail = this.authService.userEmail;
    if (!userEmail) {
      this.notificationService.handleError({ message: 'User email not found' });
      return;
    }

    this.isLoading = true;

    // First, get the current user's employee record
    this.apiService.getEmployeeByEmail({ email: userEmail }).subscribe({
      next: (employee) => {
        this.currentUserId = employee.id;
        this.loadTeamAttendance();
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  loadTeamAttendance(): void {
    if (!this.currentUserId) return;

    // Get all departments to find ones managed by the current user
    this.apiService.getDepartmentsWithPagination({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (response) => {
        // Filter departments where the manager ID equals the current user's ID
        const managedDepartments = response.items.filter(
          dept => dept.manager?.id === this.currentUserId
        );

        if (managedDepartments.length === 0) {
          this.isLoading = false;
          this.departmentName = 'No Managed Departments';
          return;
        }

        // If managing multiple departments, get overview for each and aggregate
        const overviewRequests = managedDepartments.map(dept => 
          this.apiService.getDepartmentOverview(dept.id)
        );

        forkJoin(overviewRequests).subscribe({
          next: (overviews) => {
            // Aggregate data from all managed departments
            this.teamMembers = overviews.flatMap(overview => overview.teamMembers);
            this.presentCount = overviews.reduce((sum, overview) => sum + overview.presentToday, 0);
            this.absentCount = overviews.reduce((sum, overview) => sum + overview.absentToday, 0);
            this.lateCount = overviews.reduce((sum, overview) => sum + overview.lateToday, 0);
            this.onLeaveCount = overviews.reduce((sum, overview) => sum + overview.onLeaveToday, 0);
            
            // Set department name(s)
            if (managedDepartments.length === 1) {
              this.departmentName = overviews[0].name;
            } else {
              this.departmentName = `${managedDepartments.length} Departments`;
            }
            
            this.isLoading = false;
          },
          error: (err) => {
            this.notificationService.handleError(err);
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'present': return 'check_circle';
      case 'absent': return 'cancel';
      case 'late': return 'schedule';
      case 'on-leave': return 'event_available';
      default: return 'help';
    }
  }

  getStatusClass(status: string): string {
    return `status-${status}`;
  }
}
