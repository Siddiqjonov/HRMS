import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { AbsenceRequestBriefResponse, GetAbsenceRequestsWithPaginationRequest } from '../../../../core/models/response-models/absence-request-request.model';
import { RequestStatus } from '../../../../core/models/enums/request-status.enum';
import { RequestType } from '../../../../core/models/enums/request-type.enum';
import { ProcessAbsenceRequestDialogComponent, ProcessAbsenceRequestDialogData } from '../../../absence-requests/process-absence-request-dialog/process-absence-request-dialog.component';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

@Component({
  selector: 'app-pending-approvals-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, MatBadgeModule, WidgetSkeletonComponent],
  templateUrl: './pending-approvals-widget.component.html',
  styleUrl: './pending-approvals-widget.component.css'
})
export class PendingApprovalsWidgetComponent implements OnInit, OnChanges, OnDestroy {
  @Input() employeeId: string | null = null;
  @Output() approvalProcessed = new EventEmitter<void>();

  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private dialog = inject(MatDialog);
  private refreshSubscription?: Subscription;

  approvals: AbsenceRequestBriefResponse[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadPendingApprovals();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'pending-approvals') {
        this.loadPendingApprovals();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employeeId'] && this.employeeId) {
      this.loadPendingApprovals();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadPendingApprovals(): void {
    if (!this.employeeId) return;

    this.isLoading = true;

    // Build request to get pending requests where current user is the manager/approver
    const request: GetAbsenceRequestsWithPaginationRequest = {
      pageNumber: 1,
      pageSize: 10,
      managerId: this.employeeId,
      status: RequestStatus.Pending
    };

    this.apiService.getAbsenceRequestsWithPagination(request).subscribe({
      next: (response) => {
        // Now we get only requests where current user is the approver
        this.approvals = response.items;
        this.isLoading = false;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
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

  onApprove(id: string): void {
    const approval = this.approvals.find(a => a.id === id);
    if (!approval) return;

    const dialogRef = this.dialog.open(ProcessAbsenceRequestDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      data: { 
        request: approval, 
        approved: true 
      } as ProcessAbsenceRequestDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload the list to reflect the changes
        this.loadPendingApprovals();
        this.approvalProcessed.emit();
      }
    });
  }

  onReject(id: string): void {
    const approval = this.approvals.find(a => a.id === id);
    if (!approval) return;

    const dialogRef = this.dialog.open(ProcessAbsenceRequestDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      data: { 
        request: approval, 
        approved: false 
      } as ProcessAbsenceRequestDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload the list to reflect the changes
        this.loadPendingApprovals();
        this.approvalProcessed.emit();
      }
    });
  }
}

