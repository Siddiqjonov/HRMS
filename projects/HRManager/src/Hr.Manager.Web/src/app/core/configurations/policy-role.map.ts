import { UserRole } from "../models/roles.model";

export const    PolicyRoleMap: Record<string, UserRole[]> = {
    RequireAdmin: [UserRole.Admin],
    RequireHrManager: [UserRole.HrManager],
    RequireEmployee: [UserRole.Employee],
    RequireAdminOrHrManager: [UserRole.Admin, UserRole.HrManager]
};
