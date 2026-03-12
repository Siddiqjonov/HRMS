import { inject } from "@angular/core";
import { AuthenticationService } from "../services/authentication.service";
import { CanActivateFn, Router } from "@angular/router";
import { UserRole } from "../models/roles.model";

export const AuthGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthenticationService);
    const router = inject(Router);

    const requiredRoles = route.data['roles'] as UserRole[] | undefined;

    if (requiredRoles && !requiredRoles.some(role => authService.hasRole(role))) {
        router.navigate(['/unauthorized']);
        return false;
    }
    return true;
};