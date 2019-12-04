import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
    providedIn: 'root'
})

export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

    canActivate(next: ActivatedRouteSnapshot): boolean {

        // Check if the user is logged in.
        if (!this.authService.loggedIn()) {
            this.alertify.error('You are required to log in to access this page.');
            // this.router.navigate(['/home']);
            return false;
        }

        // Check if the user needs a specific role to access the page
        const roles = next.firstChild.data[`roles`] as Array<string>;
        if (roles) {
            if (!this.authService.roleMatch(roles)) {
                this.alertify.error('You are not authorised for this page.');
                // this.router.navigate(['/members']);
                return false;
            }
        }

        return true;
    }
}
