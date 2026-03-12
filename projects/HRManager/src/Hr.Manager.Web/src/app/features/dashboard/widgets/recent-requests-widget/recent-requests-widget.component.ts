import { Component, Input, OnInit, OnChanges, SimpleChanges, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { AbsenceRequestBriefResponse, GetAbsenceRequestsWithPaginationRequest } from '../../../../core/models/response-models/absence-request-request.model';
import { RequestStatus } from '../../../../core/models/enums/request-status.enum';
import { RequestType } from '../../../../core/models/enums/request-type.enum';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

@Component({
  selector: 'app-recent-requests-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, WidgetSkeletonComponent],
  templateUrl: './recent-requests-widget.component.html',
  styleUrl: './recent-requests-widget.component.css'
})
export class RecentRequestsWidgetComponent implements OnInit, OnChanges, OnDestroy {
  @Input() employeeId: string | null = null;

  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  requests: AbsenceRequestBriefResponse[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadRecentRequests();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'recent-requests') {
        this.loadRecentRequests();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employeeId'] && this.employeeId) {
      this.loadRecentRequests();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadRecentRequests(): void {
    if (!this.employeeId) return;

    this.isLoading = true;

    // Build request with only non-null values
    const request: any = {
      pageNumber: 1,
      pageSize: 10,
      employeeId: this.employeeId
    };

    this.apiService.getAbsenceRequestsWithPagination(request).subscribe({
      next: (response) => {
        // Sort by most recent and limit to 5
        this.requests = response.items
          .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())
          .slice(0, 5);
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
      day: 'numeric',
      year: 'numeric'
    });
  }

  getRequestIcon(type: RequestType): string {
    switch (type) {
      case RequestType.Vacation: return 'event_available';
      case RequestType.Sick: return 'medical_services';
      case RequestType.Remote: return 'home';
      case RequestType.Unpaid: return 'event_busy';
      default: return 'assignment';
    }
  }
}

