import { Routes } from '@angular/router';
import { EmployeeListComponent } from './features/employees/employee-list/employee-list.component';
import { MyAbsenceListComponent } from './features/absence-requests/my-absence-list/my-absence-list.component';
import { MyAbsenceComponent } from './features/absence-requests/my-absence/my-absence.component';
import { AuthGuard } from './core/guards/auth.guard';
import { MyAttendance } from './features/attendance/my-attendance/my-attendance';

export const routes: Routes = [
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard-routes').then(m => m.routes) },
    { path: 'employees', loadChildren: () => import('./features/employees/employee-routes').then(m => m.routes) },
    { path: 'departments', loadChildren: () => import('./features/departments/department-routes').then(m => m.routes) },
    { path: 'schedules', loadChildren: () => import('./features/schedules/schedule-routes').then(m => m.routes) },
    { path: 'positions', loadChildren: () => import('./features/positions/position-routes').then(m => m.routes) },
    { path: 'absences', loadChildren: () => import('./features/absence-requests/absence-request-routes').then(m => m.routes) },
    { path: 'documents', loadChildren: () => import('./features/documents/document-routes').then(m => m.routes) },
    { 
        path: 'my-absences', 
        component: MyAbsenceListComponent,
        canActivate: [AuthGuard],
        data: { roles: ['Employee'] }
    },
    { 
        path: 'my-absences/new', 
        component: MyAbsenceComponent,
        canActivate: [AuthGuard],
        data: { roles: ['Employee'] }
    },
    { path: 'attendance', loadChildren: () => import('./features/attendance/attendance-routes').then(m => m.routes) },
    { 
        path: 'my-attendance', 
        component: MyAttendance,
        canActivate: [AuthGuard],
        data: { roles: ['Employee', 'HrManager'] }
    },
    { path: 'reports', loadChildren: () => import('./features/reports/report-routes').then(m => m.routes) },
];
