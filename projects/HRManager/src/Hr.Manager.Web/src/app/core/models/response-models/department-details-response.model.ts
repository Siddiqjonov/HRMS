import { EmployeesBriefResponse } from "./employee-response.model";

export interface DepartmentDetailsResponse {
    id: string;
    name: string;
    description: string;
    manager?: EmployeesBriefResponse | null;
}