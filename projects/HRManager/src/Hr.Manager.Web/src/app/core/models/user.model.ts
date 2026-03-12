import { UserRole } from "./roles.model";

export interface User {
    name: string;
    email?: string;
    username?: string;
    role: UserRole;
}
