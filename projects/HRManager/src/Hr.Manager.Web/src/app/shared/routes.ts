export const routes = {
    absenceRequests: {
        create: 'AbsenceRequests/CreateAbsenceRequest',
        process: 'AbsenceRequests/ProcessAbsenceRequest',
        getWithPagination: 'AbsenceRequests/GetAbsenceRequestsWithPagination',
    },

    attendance: {
        checkIn: 'Attendance/CheckIn',
        checkOut: 'Attendance/CheckOut',
        correct: 'Attendance/Correct',
        getWithFilter: 'Attendance/Records',
    },

    departments: {
        getWithPagination: 'Departments/GetDepartmentsWithPagination',
        create: 'Departments/CreateDepartment',
        update: 'Departments/UpdateDepartment',
        delete: (id: string) => `Departments/DeleteDepartment/${id}`,
        getById: (id: string) => `Departments/GetDepartmentById/${id}`,
        getOverview: (id: string) => `Departments/GetDepartmentOverview/${id}`,
        getEmployees: (id: string) => `Departments/GetDepartmentEmployees/${id}`,
        assignManager: 'Departments/AssignManager',
        removeManager: (id: string) => `Departments/RemoveManager/${id}`,
    },

    employeeDocuments: {
        upload: 'EmployeeDocuments/UploadDocument',
        getAll: 'EmployeeDocuments/GetDocuments',
        getDownloadUrl: (documentId: string) => `EmployeeDocuments/${documentId}/GetDownloadUrl`,
        delete: (documentId: string) => `EmployeeDocuments/${documentId}/DeleteDocument`,
    },

    employees: {
        getWithPagination: 'Employees/GetEmployeesWithPagination',
        create: 'Employees/CreateEmployee',
        getById: (id: string) => `Employees/GetEmployeeById/${id}`,
        update: 'Employees/UpdateEmployee',
        delete: (id: string) => `Employees/DeleteEmployee/${id}`,
        getByEmail: (email: string) => `Employees/GetEmployeeByEmail/${email}`,
        getOverview: 'Employees/GetEmployeeOverview',
    },

    position: {
        getById: (id: string) => `Position/GetPositionById/${id}`,
        create: 'Position/CreatePosition',
        update: 'Position/UpdatePosition',
        delete: (id: string) => `Position/DeletePosition/${id}`,
        getByDepartmentId: (departmentId: string) => `Position/GetPositionsByDepartmentId/${departmentId}`,
        getPositionEmployees: (positionId: string) => `Position/GetPositionEmployees/${positionId}`,
        getWithPagination: 'Position/GetPositionsWithPagination',
    },

    reports: {
        exportEmployeeList: 'Reports/ExportEmployeeList',
        departmentsSummary: 'Reports/Departments/Summary',
        attendanceSummary: 'Reports/Attendance/Summary',
        lateArrivals: 'Reports/Attendance/LateArrivals',
        overtime: 'Reports/Attendance/Overtime',
        attendancePercentage: 'Reports/Attendance/Percentage',
    },

    schedule: {
        getById: (id: string) => `Schedule/GetScheduleById/${id}`,
        create: 'Schedule/CreateSchedule',
        update: `Schedule/UpdateSchedule`,
        delete: (id: string) => `Schedule/DeleteSchedule/${id}`,
        getWithPagination: 'Schedule/GetSchedulesWithPagination',
        getScheduleEmployees: (scheduleId: string) => `Schedule/GetScheduleEmployees/${scheduleId}`,
    },
};
