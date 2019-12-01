import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject } from 'rxjs';

import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
    providedIn: 'root'
})

export class AuthService {

    baseUrl = environment.apiUrl + 'auth/';
    jwtHelper = new JwtHelperService();

    decodedToken: any;
    currentUser: User;
    photoUrl = new BehaviorSubject<string>('../../assets/user.png');
    currentPhotoUrl = this.photoUrl.asObservable();

    constructor(private http: HttpClient) {}

    // Set the main photo in the observable, exposing it to the whole app.
    changeMemberPhoto(photoUrl: string) {
        this.photoUrl.next(photoUrl);
    }

    login(model: any) {
        return this.http.post(this.baseUrl + 'login', model)
            .pipe(
                map((response: any) => {
                    const user = response;
                    if (user) {
                        localStorage.setItem('token', user.token);
                        localStorage.setItem('user', JSON.stringify(user.user));
                        this.decodedToken = this.jwtHelper.decodeToken(user.token);
                        this.currentUser = user.user;

                        // Set the new photo after first login.
                        this.changeMemberPhoto(this.currentUser.mainPhotoUrl);
                    }
                })
            );
    }

    register(user: User) {
        return this.http.post(this.baseUrl + 'register', user);
    }

    // Check login status by checking if our local token is expired yet
    loggedIn() {
        const token = localStorage.getItem('token');
        return !this.jwtHelper.isTokenExpired(token);
    }
}
