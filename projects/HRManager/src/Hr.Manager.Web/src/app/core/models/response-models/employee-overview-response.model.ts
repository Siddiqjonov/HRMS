export interface EmployeeOverviewResponse {
  statistics: EmployeeStatistics;
  attendance: CompanyAttendance;
  upcomingBirthdays: UpcomingBirthday[];
}

export interface EmployeeStatistics {
  totalEmployees: number;
  activeEmployees: number;
  newHiresThisMonth: number;
  averageTenure: number;
  turnoverRate: number;
}

export interface CompanyAttendance {
  totalEmployees: number;
  present: number;
  absent: number;
  onLeave: number;
  late: number;
  remoteWorking: number;
}

export interface UpcomingBirthday {
  id: string;
  employeeName: string;
  department: string;
  date: string;
  age: number;
}
