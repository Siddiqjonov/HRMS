import { NavigationItem } from "../models/navigation-item.model";
import { UserRole } from "../models/roles.model";

export const NAVIGATION_ITEMS: NavigationItem[] = [
  {
    label: 'Dashboard',
    icon: 'dashboard',
    route: '/dashboard',
    roles: [UserRole.Admin, UserRole.HrManager, UserRole.Employee]
  },
  {
    label: 'Employees',
    icon: 'group',
    route: '/employees',
    roles: [UserRole.Admin, UserRole.HrManager]
  },
  {
    label: 'Departments',
    icon: 'business',
    route: '/departments',
    roles: [UserRole.Admin, UserRole.HrManager]
  },
  {
    label: 'Positions',
    icon: 'work',
    route: '/positions',
    roles: [UserRole.Admin, UserRole.HrManager]
  },
  {
    label: 'Attendance Management',
    icon: 'access_time',
    route: '/attendance',
    roles: [UserRole.Admin, UserRole.HrManager],
    children: [
      {
        label: 'Attendance',
        icon: 'schedule',
        route: '/attendance',
        roles: [UserRole.HrManager]
      },
      {
        label: 'My Attendance',
        icon: 'schedule',
        route: '/my-attendance',
        roles: [UserRole.Employee]
      }
    ]
  },
  {
    label: 'Absence Management',
    icon: 'beach_access',
    route: '/absences',
    roles: [UserRole.Admin, UserRole.HrManager],
    children: [
      {
        label: 'Absences',
        icon: 'beach_access',
        route: '/absences',
        roles: [UserRole.HrManager]
      },
      {
        label: 'My Absences',
        icon: 'beach_access',
        route: '/my-absences',
        roles: [UserRole.Employee]
      }

    ]
  },
  {
    label: 'Schedules',
    icon: 'event',
    route: '/schedules',
    roles: [UserRole.Admin, UserRole.HrManager]
  },
  {
    label: 'Documents',
    icon: 'folder',
    route: '/documents',
    roles: [UserRole.Admin, UserRole.HrManager]
  },
  {
    label: 'Reports',
    icon: 'assessment',
    route: '/reports',
    roles: [UserRole.Admin, UserRole.HrManager]
  }
];
