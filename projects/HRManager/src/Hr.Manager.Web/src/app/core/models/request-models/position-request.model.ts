export interface CreatePositionRequest {
    title: string;
    departmentId: string;
    salaryMin: number;
    salaryMax: number;
}
export interface UpdatePositionRequest {
    id: string;
    title: string;
    departmentId: string;
    salaryMin: number;
    salaryMax: number;
}

export interface DeletePositionRequest {
    id: string;
}

export interface GetPositionByIdRequest {
    id: string;
}

export interface GetPositionEmployeesRequest {
    positionId: string;
}

export interface GetPositionsByDepartmentIdRequest {
    departmentId: string;
}
