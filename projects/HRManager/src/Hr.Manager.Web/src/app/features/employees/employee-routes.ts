import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { EmployeeListComponent } from "./employee-list/employee-list.component";
import { EmployeeDetailsDialogComponent } from "./employee-details-dialog/employee-details-dialog.component";
import { EmployeeFormComponent } from "./employee-form/employee-form.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['Admin', 'HrManager'] },
        children: [
            { path: '', component: EmployeeListComponent },
            { path: 'new', component: EmployeeFormComponent },
            { path: ':id/edit', component: EmployeeFormComponent }
        ]
    }
]