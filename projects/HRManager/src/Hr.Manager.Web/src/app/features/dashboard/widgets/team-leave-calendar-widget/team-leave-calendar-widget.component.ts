import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Subscription, forkJoin } from 'rxjs';
import { LeaveCalendarItem } from '../../../../core/models/response-models/department-overview-response.model';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

@Component({
  selector: 'app-team-leave-calendar-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './team-leave-calendar-widget.component.html',
  styleUrl: './team-leave-calendar-widget.component.css'
})
export class TeamLeaveCalendarWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private authService = inject(AuthenticationService);
  private refreshSubscription?: Subscription;
  
  private currentUserId: string | null = null;

  leaves: LeaveCalendarItem[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadCurrentUserAndLeaves();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'team-leave-calendar') {
        this.loadCurrentUserAndLeaves();
      }
    });
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadCurrentUserAndLeaves(): void {
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
        this.loadTeamLeaves();
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  loadTeamLeaves(): void {
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
          this.leaves = [];
          return;
        }

        // Get overview for each managed department
        const overviewRequests = managedDepartments.map(dept => 
          this.apiService.getDepartmentOverview(dept.id)
        );

        forkJoin(overviewRequests).subscribe({
          next: (overviews) => {
            // Aggregate upcoming leaves from all managed departments
            this.leaves = overviews
              .flatMap(overview => overview.upcomingLeaves || [])
              .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime());
            
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

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric'
    });
  }

  getDateRange(start: string, end: string): string {
    return `${this.formatDate(start)} - ${this.formatDate(end)}`;
  }

  getLeaveColor(type: string): string {
    const colors: { [key: string]: string } = {
      'vacation': '#2196f3',
      'sick': '#f44336',
      'personal': '#9c27b0',
      'annual': '#4caf50',
      'unpaid': '#ff9800',
      'remote': '#00bcd4'
    };
    return colors[type.toLowerCase()] || '#757575';
  }
}

