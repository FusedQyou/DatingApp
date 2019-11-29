import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

    title = 'Dating App';
    model: any = {};
    mainPhotoUrl: string;

    constructor(
        public authService: AuthService,
        private alertify: AlertifyService,
        private router: Router
    ) {}

    ngOnInit() {

        // Subscribe to the photo url service that will supply our main photo, since the navbar is not associated with the page that edits this.
        this.authService.currentPhotoUrl.subscribe(photoUrl => this.mainPhotoUrl = photoUrl);
    }

    login() {
        this.authService.login(this.model).subscribe(next => {
            this.alertify.success('Logged in successfully.');
        }, error => {
            this.alertify.error(error);
        }, () => {
            this.router.navigate(['/members']);
        });
    }

    loggedIn() {
        return this.authService.loggedIn();
    }

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        this.authService.decodedToken = null;
        this.authService.currentUser = null;
        this.alertify.success('Logged out successfully.');
        this.router.navigate(['/home']);
    }
}
