import { Routes } from '@angular/router';
import { ReportsListComponent } from './reports-list/reports-list.component';
import { AuthGuard } from '../../core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        component: ReportsListComponent,
        canActivate: [AuthGuard],
        data: { roles: ['HrManager', 'Employee'] }
    }
];
