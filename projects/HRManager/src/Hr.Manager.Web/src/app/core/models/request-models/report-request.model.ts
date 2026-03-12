export interface ExportEmployeeListRequest {
    departmentId?: string | null;
    startDate?: string | null;
    endDate?: string | null;
}

export interface GetAttendanceSummaryReportRequest {
    startDate?: string | null;
    endDate?: string | null;
    departmentId?: string | null;
}

export interface GetLateArrivalReportRequest {
    startDate?: string | null;
    endDate?: string | null;
    departmentId?: string | null;
    employeeId?: string | null;
}

export interface GetOvertimeReportRequest {
    startDate?: string | null;
    endDate?: string | null;
    departmentId?: string | null;
    employeeId?: string | null;
}

export interface GetAttendancePercentageReportRequest {
    startDate?: string | null;
    endDate?: string | null;
    departmentId?: string | null;
}

