import { inject, Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { routes } from '../../shared/routes';
import { PaginatedModel } from '../models/common/paginated.model';
import { AbsenceRequestBriefResponse, GetAbsenceRequestsWithPaginationRequest } from '../models/response-models/absence-request-request.model';
import { Observable } from 'rxjs';
import { EmployeeDocumentsResponse, GetDocumentDownloadUrlResponse } from '../models/response-models/employee-document-response.model';
import { EmployeeResponse, EmployeesBriefResponse } from '../models/response-models/employee-response.model';
import { EmployeeOverviewResponse } from '../models/response-models/employee-overview-response.model';
import { PositionResponse } from '../models/response-models/position-response.model';
import { ScheduleResponse } from '../models/response-models/schedule-response.model';
import { PaginationRequest } from '../models/common/pagination-request.model';
import { AssignDepartmentManagerRequest, CreateDepartmentRequest, DeleteDepartmentRequest, GetDepartmentByIdRequest, RemoveDepartmentManagerRequest, UpdateDepartmentRequest } from '../models/request-models/departmet-request.model';
import { DeleteEmployeeDocumentRequest, GetDocumentDownloadUrlRequest, GetEmployeeDocumentsRequest, UploadEmployeeDocumentRequest } from '../models/request-models/employee-documents-request.model';
import { DepartmentDetailsResponse } from '../models/response-models/department-details-response.model';
import { DepartmentOverviewResponse } from '../models/response-models/department-overview-response.model';
import { CreateEmployeeRequest, DeleteEmployeeRequest, GetEmployeeByEmailRequest, GetEmployeeByIdRequest, UpdateEmployeeRequest } from '../models/request-models/employee-request.model';
import { CreatePositionRequest, DeletePositionRequest, GetPositionByIdRequest, GetPositionEmployeesRequest, UpdatePositionRequest } from '../models/request-models/position-request.model';
import { ExportEmployeeListRequest, GetAttendancePercentageReportRequest, GetAttendanceSummaryReportRequest, GetLateArrivalReportRequest, GetOvertimeReportRequest } from '../models/request-models/report-request.model';
import { CreateScheduleRequest, GetScheduleByIdRequest, GetScheduleEmployeesRequest, UpdateScheduleRequest } from '../models/request-models/schedule-request.model';
import { HttpResponse } from '@angular/common/http';
import { CreateAbsenceRequest, ProcessAbsenceRequest } from '../models/request-models/absence-request.model';
import { CheckInRequest, CheckOutRequest, CorrectAttendanceRecordRequest, GetAttendanceRecordsRequest } from '../models/request-models/attendance-request.models';
import { GetAttendanceRecordsResponse } from '../models/response-models/attendance-response.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private httpService = inject(HttpService);

  getAttendanceRecordsWithFilter(request: GetAttendanceRecordsRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<GetAttendanceRecordsResponse>> {
    return this.httpService.get<PaginatedModel<GetAttendanceRecordsResponse>>(routes.attendance.getWithFilter, { params: request });
  }

  getAbsenceRequestsWithPagination(request: GetAbsenceRequestsWithPaginationRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<AbsenceRequestBriefResponse>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {
      pageNumber: request.pageNumber,
      pageSize: request.pageSize
    };
    
    if (request.employeeId != null) {
      cleanParams.employeeId = request.employeeId;
    }
    if (request.managerId != null) {
      cleanParams.managerId = request.managerId;
    }
    if (typeof request.status === 'number') {
      cleanParams.status = request.status;
    }
    
    if (typeof request.type === 'number') {
      cleanParams.type = request.type;
    }
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    
    return this.httpService.get<PaginatedModel<AbsenceRequestBriefResponse>>(routes.absenceRequests.getWithPagination, { params: cleanParams });
  }

  createAbsenceRequest(request: CreateAbsenceRequest): Observable<void> {
    return this.httpService.post<void>(routes.absenceRequests.create, request);
  }

  processAbsenceRequest(request: ProcessAbsenceRequest): Observable<void> {
    return this.httpService.put<void>(routes.absenceRequests.process, request);
  }

  checkIn(request: CheckInRequest): Observable<void> {
    return this.httpService.post<void>(routes.attendance.checkIn, request);
  }

  checkOut(request: CheckOutRequest): Observable<void> {
    return this.httpService.post<void>(routes.attendance.checkOut, request);
  }

  correctAttendance(request: CorrectAttendanceRecordRequest): Observable<void> {
    return this.httpService.put<void>(routes.attendance.correct, request);
  }

  getDepartmentsWithPagination(request: PaginationRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<DepartmentDetailsResponse>> {
    return this.httpService.get<PaginatedModel<DepartmentDetailsResponse>>(routes.departments.getWithPagination, { params: request });
  }

  createDepartment(request: CreateDepartmentRequest): Observable<void> {
    return this.httpService.post<void>(routes.departments.create, request);
  }

  updateDepartment(request: UpdateDepartmentRequest): Observable<void> {
    return this.httpService.put<void>(routes.departments.update, request);
  }

  deleteDepartment(request: DeleteDepartmentRequest): Observable<void> {
    return this.httpService.delete<void>(routes.departments.delete(request.id));
  }

  getDepartmentById(request: GetDepartmentByIdRequest): Observable<DepartmentDetailsResponse> {
    return this.httpService.get<DepartmentDetailsResponse>(routes.departments.getById(request.id));
  }

  getDepartmentOverview(departmentId: string): Observable<DepartmentOverviewResponse> {
    return this.httpService.get<DepartmentOverviewResponse>(routes.departments.getOverview(departmentId));
  }

  assignDepartmentManager(request: AssignDepartmentManagerRequest): Observable<void> {
    return this.httpService.post<void>(routes.departments.assignManager, request);
  }

  removeDepartmentManager(request: RemoveDepartmentManagerRequest): Observable<void> {
    return this.httpService.delete<void>(routes.departments.removeManager(request.departmentId));
  }

  getDepartmentEmployees(departmentId: string): Observable<EmployeesBriefResponse[]> {
    return this.httpService.get<EmployeesBriefResponse[]>(routes.departments.getEmployees(departmentId));
  }

  uploadEmployeeDocument(request: UploadEmployeeDocumentRequest): Observable<EmployeeDocumentsResponse> {
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('file', request.file, request.file.name);
    formData.append('documentType', request.documentType.toString());

    return this.httpService.post<EmployeeDocumentsResponse>(routes.employeeDocuments.upload, formData);
  }

  getDocuments(request: GetEmployeeDocumentsRequest): Observable<PaginatedModel<EmployeeDocumentsResponse>> {
    // Clean params - remove null and undefined values
    const cleanParams: any = {
      pageNumber: request.pageNumber,
      pageSize: request.pageSize
    };
    
    if (request.employeeId != null) {
      cleanParams.employeeId = request.employeeId;
    }
    if (typeof request.documentType === 'number') {
      cleanParams.documentType = request.documentType;
    }
    
    return this.httpService.get<PaginatedModel<EmployeeDocumentsResponse>>(routes.employeeDocuments.getAll, { params: cleanParams });
  }

  getDocumentDownloadUrl(request: GetDocumentDownloadUrlRequest): Observable<GetDocumentDownloadUrlResponse> {
    return this.httpService.get<GetDocumentDownloadUrlResponse>(routes.employeeDocuments.getDownloadUrl(request.documentId));
  }

  deleteEmployeeDocument(request: DeleteEmployeeDocumentRequest): Observable<void> {
    return this.httpService.delete<void>(routes.employeeDocuments.delete(request.documentId)
    );
  }

  getEmployeesWithPagination(request: PaginationRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<EmployeesBriefResponse>> {
    return this.httpService.get<PaginatedModel<EmployeesBriefResponse>>(routes.employees.getWithPagination, { params: request });
  }

  getEmployeeById(request: GetEmployeeByIdRequest): Observable<EmployeeResponse> {
    return this.httpService.get<EmployeeResponse>(routes.employees.getById(request.id));
  }

  getEmployeeByEmail(request: GetEmployeeByEmailRequest): Observable<EmployeeResponse> {
    return this.httpService.get<EmployeeResponse>(routes.employees.getByEmail(request.email));
  }

  createEmployee(request: CreateEmployeeRequest): Observable<void> {
    return this.httpService.post<void>(routes.employees.create, request);
  }

  updateEmployee(request: UpdateEmployeeRequest): Observable<void> {
    return this.httpService.put<void>(routes.employees.update, request);
  }

  deleteEmployee(request: DeleteEmployeeRequest): Observable<void> {
    return this.httpService.delete<void>(routes.employees.delete(request.id));
  }

  getEmployeeOverview(): Observable<EmployeeOverviewResponse> {
    return this.httpService.get<EmployeeOverviewResponse>(routes.employees.getOverview);
  }

  getPositionById(request: GetPositionByIdRequest): Observable<PositionResponse> {
    return this.httpService.get<PositionResponse>(routes.position.getById(request.id));
  }

  createPosition(request: CreatePositionRequest): Observable<void> {
    return this.httpService.post<void>(routes.position.create, request);
  }

  updatePosition(request: UpdatePositionRequest): Observable<void> {
    return this.httpService.put<void>(routes.position.update, request);
  }

  deletePosition(request: DeletePositionRequest): Observable<void> {
    return this.httpService.delete<void>(routes.position.delete(request.id));
  }

  getPositionsByDepartmentId(departmentId: string): Observable<PositionResponse[]> {
    return this.httpService.get<PositionResponse[]>(routes.position.getByDepartmentId(departmentId));
  }

  getPositionEmployees(request: GetPositionEmployeesRequest): Observable<EmployeesBriefResponse[]> {
    return this.httpService.get<EmployeesBriefResponse[]>(routes.position.getPositionEmployees(request.positionId));
  }

  getPositionsWithPagination(request: PaginationRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<PositionResponse>> {
    return this.httpService.get<PaginatedModel<PositionResponse>>(routes.position.getWithPagination, { params: request });
  }

  exportEmployeeList(request: ExportEmployeeListRequest): Observable<HttpResponse<Blob>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {};
    
    if (request.departmentId != null) {
      cleanParams.departmentId = request.departmentId;
    }
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    
    return this.httpService.get(routes.reports.exportEmployeeList, { params: cleanParams, responseType: 'blob' as 'json', observe: 'response' });
  }

  getDepartmentsSummary(departmentId?: string | null): Observable<HttpResponse<Blob>> {
    const params: any = {};
    if (departmentId != null) {
      params.departmentId = departmentId;
    }
    return this.httpService.get(routes.reports.departmentsSummary, { params, responseType: 'blob' as 'json', observe: 'response' });
  }

  getAttendanceSummaryReport(request: GetAttendanceSummaryReportRequest = {}): Observable<HttpResponse<Blob>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {};
    
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    if (request.departmentId != null) {
      cleanParams.departmentId = request.departmentId;
    }
    
    return this.httpService.get(routes.reports.attendanceSummary, { params: cleanParams, responseType: 'blob' as 'json', observe: 'response' });
  }

  getLateArrivalReport(request: GetLateArrivalReportRequest = {}): Observable<HttpResponse<Blob>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {};
    
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    if (request.departmentId != null) {
      cleanParams.departmentId = request.departmentId;
    }
    if (request.employeeId != null) {
      cleanParams.employeeId = request.employeeId;
    }
    
    return this.httpService.get(routes.reports.lateArrivals, { params: cleanParams, responseType: 'blob' as 'json', observe: 'response' });
  }

  getOvertimeReport(request: GetOvertimeReportRequest = {}): Observable<HttpResponse<Blob>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {};
    
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    if (request.departmentId != null) {
      cleanParams.departmentId = request.departmentId;
    }
    if (request.employeeId != null) {
      cleanParams.employeeId = request.employeeId;
    }
    
    return this.httpService.get(routes.reports.overtime, { params: cleanParams, responseType: 'blob' as 'json', observe: 'response' });
  }

  getAttendancePercentageReport(request: GetAttendancePercentageReportRequest = {}): Observable<HttpResponse<Blob>> {
    // Filter out null and undefined values from params
    const cleanParams: any = {};
    
    if (request.startDate != null) {
      cleanParams.startDate = request.startDate;
    }
    if (request.endDate != null) {
      cleanParams.endDate = request.endDate;
    }
    if (request.departmentId != null) {
      cleanParams.departmentId = request.departmentId;
    }
    
    return this.httpService.get(routes.reports.attendancePercentage, { params: cleanParams, responseType: 'blob' as 'json', observe: 'response' });
  }

  getScheduleById(request: GetScheduleByIdRequest): Observable<ScheduleResponse> {
    return this.httpService.get<ScheduleResponse>(routes.schedule.getById(request.id));
  }

  createSchedule(request: CreateScheduleRequest): Observable<ScheduleResponse> {
    return this.httpService.post<ScheduleResponse>(routes.schedule.create, request);
  }

  updateSchedule(request: UpdateScheduleRequest): Observable<void> {
    return this.httpService.put<void>(routes.schedule.update, request);
  }

  deleteSchedule(request: { id: string }): Observable<void> {
    return this.httpService.delete<void>(routes.schedule.delete(request.id));
  }

  getSchedulesWithPagination(request: PaginationRequest = { pageNumber: 1, pageSize: 10 }): Observable<PaginatedModel<ScheduleResponse>> {
    return this.httpService.get<PaginatedModel<ScheduleResponse>>(routes.schedule.getWithPagination, { params: request });
  }

  getScheduleEmployees(request: GetScheduleEmployeesRequest): Observable<EmployeesBriefResponse[]> {
    return this.httpService.get<EmployeesBriefResponse[]>(routes.schedule.getScheduleEmployees(request.scheduleId));
  }
}
