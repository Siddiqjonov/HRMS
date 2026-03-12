import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export type RefreshTarget = 
  | 'all'
  | 'attendance-clock'
  | 'attendance-summary'
  | 'upcoming-leaves'
  | 'recent-requests'
  | 'pending-approvals'
  | 'team-attendance'
  | 'department-overview'
  | 'team-leave-calendar'
  | 'employee-statistics'
  | 'company-attendance'
  | 'upcoming-birthdays';

@Injectable({
  providedIn: 'root'
})
export class DashboardRefreshService {
  private refreshSubject = new Subject<RefreshTarget>();
  
  // Observable that widgets can subscribe to
  refresh$ = this.refreshSubject.asObservable();
  refreshTrigger$ = this.refreshSubject.asObservable();

  /**
   * Trigger a refresh for specific widget or all widgets
   */
  triggerRefresh(target: RefreshTarget = 'all'): void {
    this.refreshSubject.next(target);
  }

  /**
   * Refresh all widgets
   */
  refreshAll(): void {
    this.triggerRefresh('all');
  }

  /**
   * Refresh attendance-related widgets
   */
  refreshAttendanceWidgets(): void {
    this.triggerRefresh('attendance-clock');
    this.triggerRefresh('attendance-summary');
    this.triggerRefresh('team-attendance');
    this.triggerRefresh('company-attendance');
  }

  /**
   * Refresh leave-related widgets
   */
  refreshLeaveWidgets(): void {
    this.triggerRefresh('upcoming-leaves');
    this.triggerRefresh('recent-requests');
    this.triggerRefresh('pending-approvals');
    this.triggerRefresh('team-leave-calendar');
  }
}

