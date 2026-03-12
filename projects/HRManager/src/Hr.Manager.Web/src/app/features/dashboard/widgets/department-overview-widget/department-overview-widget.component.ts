import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Subscription, forkJoin } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

interface DepartmentStats {
  name: string;
  totalEmployees: number;
  presentToday: number;
  onLeave: number;
  averageAttendance: number;
}

@Component({
  selector: 'app-department-overview-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './department-overview-widget.component.html',
  styleUrl: './department-overview-widget.component.css'
})
export class DepartmentOverviewWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private authService = inject(AuthenticationService);
  private refreshSubscription?: Subscription;
  
  private currentUserId: string | null = null;

  department: DepartmentStats | null = null;
  isLoading = false;

  ngOnInit(): void {
    this.loadCurrentUserAndDepartmentData();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'department-overview') {
        this.loadCurrentUserAndDepartmentData();
      }
    });
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadCurrentUserAndDepartmentData(): void {
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
        this.loadDepartmentData();
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  loadDepartmentData(): void {
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
          this.department = null;
          return;
        }

        // Get overview for each managed department
        const overviewRequests = managedDepartments.map(dept => 
          this.apiService.getDepartmentOverview(dept.id)
        );

        forkJoin(overviewRequests).subscribe({
          next: (overviews) => {
            // Aggregate data from all managed departments
            const totalEmployees = overviews.reduce((sum, overview) => sum + overview.totalEmployees, 0);
            const presentToday = overviews.reduce((sum, overview) => sum + overview.presentToday, 0);
            const onLeave = overviews.reduce((sum, overview) => sum + overview.onLeaveToday, 0);
            
            // Calculate average attendance percentage
            const averageAttendance = totalEmployees > 0 
              ? Math.round((presentToday / totalEmployees) * 100)
              : 0;
            
            // Set department stats
            this.department = {
              name: managedDepartments.length === 1 
                ? overviews[0].name 
                : `${managedDepartments.length} Departments`,
              totalEmployees,
              presentToday,
              onLeave,
              averageAttendance
            };
            
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

  get attendancePercentage(): number {
    if (!this.department || this.department.totalEmployees === 0) return 0;
    return Math.round((this.department.presentToday / this.department.totalEmployees) * 100);
  }
}

