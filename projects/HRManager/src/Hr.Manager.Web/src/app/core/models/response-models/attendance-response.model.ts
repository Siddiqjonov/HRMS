export interface GetAttendanceRecordsResponse {
    attendanceId: string;
    employeeId: string;
    employeeName : string;
    date: string;
    checkIn: string;
    checkOut: string;
    overtimeMinutes: number;
    totalMinutes: number;
    isLate: boolean;
    isEarlyDeparture: boolean;
}