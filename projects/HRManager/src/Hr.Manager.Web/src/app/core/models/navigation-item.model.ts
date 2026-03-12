import { UserRole } from "./roles.model";

export interface NavigationItem {
    label: string;
    icon: string;
    route: string;
    roles: UserRole[];
    children?: NavigationItem[];
}