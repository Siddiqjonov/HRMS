import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../../../core/services/notification.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ReportsFilterDialogComponent, ReportFilterData } from '../reports-filter-dialog/reports-filter-dialog.component';
import { MatTableDataSource } from '@angular/material/table';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { 
  ExportEmployeeListRequest, 
  GetAttendancePercentageReportRequest, 
  GetAttendanceSummaryReportRequest, 
  GetLateArrivalReportRequest, 
  GetOvertimeReportRequest 
} from '../../../core/models/request-models/report-request.model';
import { HttpResponse } from '@angular/common/http';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { UserRole } from '../../../core/models/roles.model';

interface ReportCard {
  id: string;
  title: string;
  description: string;
  icon: string;
  category: 'employee' | 'department' | 'attendance';
  requiresFilters: boolean;
  availableFilters: ('dateRange' | 'department' | 'employee')[];
}

@Component({
  selector: 'app-reports-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    LoadingSpinnerComponent,
    DataTableComponent
  ],
  templateUrl: './reports-list.component.html',
  styleUrl: './reports-list.component.css'
})
export class ReportsListComponent implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);
  private authService = inject(AuthenticationService);

  isLoading = false;
  loadingReportId: string | null = null;
  selectedReport: ReportCard | null = null;

  dataSource = new MatTableDataSource<ReportCard>([]);
  displayedColumns = [
    { key: 'select', label: 'Select' },
    { key: 'title', label: 'Report Name' },
    { key: 'description', label: 'Description' },
    { key: 'category', label: 'Category' }
  ];

  getCategoryLabelFn = (category: string): string => {
    return this.getCategoryLabel(category);
  };

  // Filter state
  startDate: Date | null = null;
  endDate: Date | null = null;
  selectedDepartment: DepartmentDetailsResponse | null = null;
  selectedEmployee: EmployeesBriefResponse | null = null;

  // Department manager state
  private isEmployeeManager = false;
  private managedDepartments: DepartmentDetailsResponse[] = [];

  reports: ReportCard[] = [
    {
      id: 'employee-list',
      title: 'Employee List',
      description: 'Export a comprehensive list of all employees with their details. Filter by department and date range.',
      icon: 'groups',
      category: 'employee',
      requiresFilters: false,
      availableFilters: ['dateRange', 'department']
    },
    {
      id: 'department-summary',
      title: 'Department Summary',
      description: 'Generate a summary report of all departments including employee counts and key metrics. Filter by specific department.',
      icon: 'domain',
      category: 'department',
      requiresFilters: false,
      availableFilters: ['department']
    },
    {
      id: 'attendance-summary',
      title: 'Attendance Summary',
      description: 'Comprehensive attendance summary showing total hours, overtime, and absences by department.',
      icon: 'summarize',
      category: 'attendance',
      requiresFilters: false,
      availableFilters: ['dateRange', 'department']
    },
    {
      id: 'late-arrivals',
      title: 'Late Arrival Report',
      description: 'Track late arrivals with detailed information about employees arriving after scheduled time.',
      icon: 'schedule',
      category: 'attendance',
      requiresFilters: false,
      availableFilters: ['dateRange', 'department', 'employee']
    },
    {
      id: 'overtime',
      title: 'Overtime Report',
      description: 'Detailed overtime hours report showing employees who worked beyond their scheduled hours.',
      icon: 'more_time',
      category: 'attendance',
      requiresFilters: false,
      availableFilters: ['dateRange', 'department', 'employee']
    },
    {
      id: 'attendance-percentage',
      title: 'Attendance Percentage',
      description: 'View attendance percentage rates by employee or department over a specified period.',
      icon: 'percent',
      category: 'attendance',
      requiresFilters: false,
      availableFilters: ['dateRange', 'department']
    }
  ];

  async ngOnInit(): Promise<void> {
    await this.checkIfEmployeeManager();
    this.updateReportFiltersForEmployeeManager();
    this.dataSource.data = this.reports;
  }

  private async checkIfEmployeeManager(): Promise<void> {
    const userRoles = this.authService.getUserRoles();
    
    // Check if user is an Employee (not Admin or HrManager)
    this.isEmployeeManager = userRoles.includes(UserRole.Employee) && 
                             !userRoles.includes(UserRole.Admin) && 
                             !userRoles.includes(UserRole.HrManager);
    
    if (!this.isEmployeeManager) {
      return;
    }

    const userEmail = this.authService.userEmail;
    if (!userEmail) return;

    try {
      // Get current user's employee record
      const employee = await this.apiService.getEmployeeByEmail({ email: userEmail }).toPromise();
      if (!employee) return;

      // Get all departments and find ones managed by this employee
      const departments = await this.apiService.getDepartmentsWithPagination({ 
        pageNumber: 1, 
        pageSize: 100 
      }).toPromise();

      if (!departments) return;

      // Find departments managed by this employee
      this.managedDepartments = departments.items.filter(
        dept => dept.manager?.id === employee.id
      );

      // If managing exactly one department, auto-select it
      if (this.managedDepartments.length === 1) {
        this.selectedDepartment = this.managedDepartments[0];
      }
    } catch (error) {
      console.error('Error checking employee manager status:', error);
    }
  }

  private updateReportFiltersForEmployeeManager(): void {
    if (!this.isEmployeeManager || this.managedDepartments.length !== 1) {
      return;
    }

    // Remove 'department' from availableFilters for all reports if managing one department
    this.reports = this.reports.map(report => ({
      ...report,
      availableFilters: report.availableFilters.filter(filter => filter !== 'department')
    }));
  }

  getCategoryLabel(category: string): string {
    switch (category) {
      case 'employee':
        return 'Employee';
      case 'department':
        return 'Department';
      case 'attendance':
        return 'Attendance';
      default:
        return category;
    }
  }

  selectReport(report: ReportCard): void {
    // Clear filters when switching to a different report
    if (this.selectedReport && this.selectedReport.id !== report.id) {
      // Store auto-set department before clearing
      const autoSetDepartment = (this.isEmployeeManager && this.managedDepartments.length === 1) 
        ? this.selectedDepartment 
        : null;
      
      this.clearAllFilters();
      
      // Restore auto-set department after clearing
      if (autoSetDepartment) {
        this.selectedDepartment = autoSetDepartment;
      }
    }
    this.selectedReport = report;
  }

  onViewReport(report: ReportCard): void {
    this.selectReport(report);
  }

  hasActiveFiltersForSelected(): boolean {
    if (!this.selectedReport) return false;
    
    if (this.selectedReport.availableFilters.includes('dateRange') && (this.startDate || this.endDate)) {
      return true;
    }
    // Only count department as active filter if it's not auto-set for employee manager
    if (this.selectedReport.availableFilters.includes('department') && this.selectedDepartment) {
      return true;
    }
    if (this.selectedReport.availableFilters.includes('employee') && this.selectedEmployee) {
      return true;
    }
    return false;
  }

  getActiveFiltersCountForSelected(): number {
    if (!this.selectedReport) return 0;
    
    let count = 0;
    if (this.selectedReport.availableFilters.includes('dateRange') && (this.startDate || this.endDate)) {
      count++;
    }
    // Only count department as active filter if it's not auto-set for employee manager
    if (this.selectedReport.availableFilters.includes('department') && this.selectedDepartment) {
      count++;
    }
    if (this.selectedReport.availableFilters.includes('employee') && this.selectedEmployee) {
      count++;
    }
    return count;
  }

  openFilterDialog(): void {
    if (!this.selectedReport) return;

    const filterData: ReportFilterData = {
      reportId: this.selectedReport.id,
      reportTitle: this.selectedReport.title,
      availableFilters: this.selectedReport.availableFilters,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      selectedDepartment: this.selectedDepartment,
      selectedEmployee: this.selectedEmployee
    };

    const dialogRef = this.dialog.open(ReportsFilterDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      data: filterData,
      disableClose: false,
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe((result: ReportFilterData | null) => {
      if (result !== null && result !== undefined) {
        // Apply filter results
        this.startDate = result.startDate ? new Date(result.startDate) : null;
        this.endDate = result.endDate ? new Date(result.endDate) : null;
        this.selectedDepartment = result.selectedDepartment || null;
        this.selectedEmployee = result.selectedEmployee || null;
      }
    });
  }

  clearAllFilters(): void {
    this.startDate = null;
    this.endDate = null;
    
    // Don't clear department if it's auto-set for employee manager with one department
    if (!(this.isEmployeeManager && this.managedDepartments.length === 1)) {
      this.selectedDepartment = null;
    }
    
    this.selectedEmployee = null;
  }

  generateSelectedReport(): void {
    if (!this.selectedReport) return;
    
    this.loadingReportId = this.selectedReport.id;
    
    switch (this.selectedReport.id) {
      case 'employee-list':
        this.generateEmployeeList();
        break;
      case 'department-summary':
        this.generateDepartmentSummary();
        break;
      case 'attendance-summary':
        this.generateAttendanceSummary();
        break;
      case 'late-arrivals':
        this.generateLateArrivalReport();
        break;
      case 'overtime':
        this.generateOvertimeReport();
        break;
      case 'attendance-percentage':
        this.generateAttendancePercentageReport();
        break;
      default:
        this.notificationService.handleError({ error: { message: 'Unknown report type' } });
        this.loadingReportId = null;
    }
  }

  private generateReport(report: ReportCard): void {
    this.loadingReportId = report.id;
  }

  private generateEmployeeList(): void {
    const request: ExportEmployeeListRequest = {
      departmentId: this.selectedDepartment?.id || null,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null
    };

    this.apiService.exportEmployeeList(request).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'EmployeeList');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private generateDepartmentSummary(): void {
    const departmentId = this.selectedDepartment?.id || null;
    
    this.apiService.getDepartmentsSummary(departmentId).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'DepartmentSummary');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private generateAttendanceSummary(): void {
    const request: GetAttendanceSummaryReportRequest = {
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      departmentId: this.selectedDepartment?.id || null
    };

    this.apiService.getAttendanceSummaryReport(request).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'AttendanceSummary');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private generateLateArrivalReport(): void {
    const request: GetLateArrivalReportRequest = {
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      departmentId: this.selectedDepartment?.id || null,
      employeeId: this.selectedEmployee?.id || null
    };

    this.apiService.getLateArrivalReport(request).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'LateArrivalReport');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private generateOvertimeReport(): void {
    const request: GetOvertimeReportRequest = {
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      departmentId: this.selectedDepartment?.id || null,
      employeeId: this.selectedEmployee?.id || null
    };

    this.apiService.getOvertimeReport(request).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'OvertimeReport');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private generateAttendancePercentageReport(): void {
    const request: GetAttendancePercentageReportRequest = {
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      departmentId: this.selectedDepartment?.id || null
    };

    this.apiService.getAttendancePercentageReport(request).subscribe({
      next: (response: HttpResponse<Blob>) => {
        this.downloadFile(response, 'AttendancePercentageReport');
        this.loadingReportId = null;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.loadingReportId = null;
      }
    });
  }

  private downloadFile(response: HttpResponse<Blob>, defaultFileName: string): void {
    if (!response.body) {
      this.notificationService.handleError({ error: { message: 'No file content received' } });
      return;
    }

    // Extract filename from Content-Disposition header or use default
    let filename = defaultFileName;
    const contentDisposition = response.headers.get('Content-Disposition');
    if (contentDisposition) {
      const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
      if (matches != null && matches[1]) {
        filename = matches[1].replace(/['"]/g, '');
      }
    }

    // Ensure filename has .xlsx extension
    if (!filename.toLowerCase().endsWith('.xlsx')) {
      // Remove any existing extension and add .xlsx
      const nameParts = filename.split('.');
      if (nameParts.length > 1) {
        nameParts.pop(); // Remove old extension
      }
      filename = nameParts.join('.') + '.xlsx';
    }

    // Add timestamp if not already present
    if (!filename.match(/_\d{4}/)) {
      const timestamp = new Date().toISOString().replace(/[:.]/g, '-').substring(0, 19).replace('T', '_');
      const extensionIndex = filename.lastIndexOf('.xlsx');
      filename = filename.substring(0, extensionIndex) + '_' + timestamp + '.xlsx';
    }

    // Create blob with proper Excel MIME type and download
    const blob = new Blob([response.body], { 
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
    });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);

    this.notificationService.success(`Report "${filename}" downloaded successfully`);
  }

  private formatDateForRequest(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
