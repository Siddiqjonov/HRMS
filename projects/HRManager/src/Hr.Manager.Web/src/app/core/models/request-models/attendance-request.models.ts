import { PaginationRequest } from "../common/pagination-request.model";

export interface CheckInRequest {
    employeeId: string;
}

export interface CheckOutRequest {
    employeeId: string;
}

export interface CorrectAttendanceRecordRequest {
    attendanceRecordId: string;
    checkIn?: string | null;
    checkOut?: string | null;
}

export interface GetAttendanceRecordsRequest extends PaginationRequest {
    employeeId?: string | null;
    startDate?: string | null;
    endDate?: string | null;
    isLate?: boolean | null;
    isEarlyDeparture?: boolean | null;
}
