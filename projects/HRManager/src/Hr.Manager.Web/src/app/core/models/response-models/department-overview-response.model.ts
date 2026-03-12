export interface DepartmentOverviewResponse {
    id: string;
    name: string;
    description: string;
    totalEmployees: number;
    presentToday: number;
    absentToday: number;
    lateToday: number;
    onLeaveToday: number;
    teamMembers: TeamMemberAttendanceInfo[];
    upcomingLeaves: LeaveCalendarItem[];
}

export interface TeamMemberAttendanceInfo {
    id: string;
    name: string;
    status: 'present' | 'absent' | 'late' | 'on-leave';
    checkInTime?: string;
    checkOutTime?: string;
}

export interface LeaveCalendarItem {
    id: string;
    employeeName: string;
    leaveType: string;
    startDate: string;
    endDate: string;
    status: string;
}

