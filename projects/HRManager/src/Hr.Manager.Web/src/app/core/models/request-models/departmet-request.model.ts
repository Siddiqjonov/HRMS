export interface AssignDepartmentManagerRequest {
    departmentId: string;
    employeeId: string;
}

export interface CreateDepartmentRequest {
    name: string;
    description: string;
    managerId?: string | null;
}

export interface DeleteDepartmentRequest {
    id: string;
}

export interface GetDepartmentByIdRequest {
    id: string;
}

export interface RemoveDepartmentManagerRequest {
    departmentId: string;
}

export interface UpdateDepartmentRequest {
    id: string;
    name: string;
    description: string;
    managerId?: string | null;
}