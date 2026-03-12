import { Component, Input, OnInit, OnChanges, SimpleChanges, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';
import { AbsenceRequestBriefResponse, GetAbsenceRequestsWithPaginationRequest } from '../../../../core/models/response-models/absence-request-request.model';
import { RequestStatus } from '../../../../core/models/enums/request-status.enum';
import { RequestType } from '../../../../core/models/enums/request-type.enum';

@Component({
  selector: 'app-upcoming-leaves-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatChipsModule, WidgetSkeletonComponent],
  templateUrl: './upcoming-leaves-widget.component.html',
  styleUrl: './upcoming-leaves-widget.component.css'
})
export class UpcomingLeavesWidgetComponent implements OnInit, OnChanges, OnDestroy {
  @Input() employeeId: string | null = null;

  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  leaves: AbsenceRequestBriefResponse[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadUpcomingLeaves();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'upcoming-leaves') {
        this.loadUpcomingLeaves();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employeeId'] && this.employeeId) {
      this.loadUpcomingLeaves();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadUpcomingLeaves(): void {
    if (!this.employeeId) return;

    this.isLoading = true;

    const request: GetAbsenceRequestsWithPaginationRequest = {
      pageNumber: 1,
      pageSize: 10,
      employeeId: this.employeeId,
      status: RequestStatus.Approved
    };

    this.apiService.getAbsenceRequestsWithPagination(request).subscribe({
      next: (response) => {
        // Filter for upcoming leaves (start date is today or in the future)
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        this.leaves = response.items.filter(leave => {
          const startDate = new Date(leave.startDate);
          startDate.setHours(0, 0, 0, 0);
          return startDate >= today;
        }).slice(0, 5); // Limit to 5 upcoming leaves
        
        this.isLoading = false;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  getStatusColor(status: RequestStatus): string {
    switch (status) {
      case RequestStatus.Approved: return 'success';
      case RequestStatus.Pending: return 'warning';
      case RequestStatus.Rejected: return 'danger';
      default: return 'default';
    }
  }

  getStatusText(status: RequestStatus): string {
    switch (status) {
      case RequestStatus.Approved: return 'Approved';
      case RequestStatus.Pending: return 'Pending';
      case RequestStatus.Rejected: return 'Rejected';
      default: return 'Unknown';
    }
  }

  getTypeText(type: RequestType): string {
    switch (type) {
      case RequestType.Vacation: return 'Vacation';
      case RequestType.Sick: return 'Sick Leave';
      case RequestType.Remote: return 'Remote Work';
      case RequestType.Unpaid: return 'Unpaid Leave';
      default: return 'Leave';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric' 
    });
  }
}

