import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { DepartmentListComponent } from "./department-list/department-list.component";
import { NewDepartmentComponent } from "./new-department/new-department.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['HrManager'] },
        children: [
            { path: '', component: DepartmentListComponent },
            {
                path: 'new',
                component: NewDepartmentComponent
            },
            {
                path: ':id/edit',
                component: NewDepartmentComponent
            }
        ]
    }
];
