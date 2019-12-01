import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {

    @Output() cancelRegister = new EventEmitter();

    model: any = {};

    registerForm: FormGroup;

    constructor( private authService: AuthService, private alertify: AlertifyService) {}
    ngOnInit() {
        this.registerForm = new FormGroup({
            username: new FormControl('', Validators.required),
            password: new FormControl('', [ Validators.required, Validators.minLength(4) ]),
            confirmPassword: new FormControl('', Validators.required)
        }, this.passwordMatchValidator);
    }

    passwordMatchValidator(group: FormGroup) {
        return group.get('password').value === group.get('confirmPassword').value ? null : {mismatch: true};
    }

    register() {
        console.log(this.registerForm.value);
        return;
        this.authService.register(this.model).subscribe(() => {
            this.alertify.success('registration successful.');
        }, error => {
            this.alertify.error(error);
        });
    }

    cancel() {
        this.cancelRegister.emit();
    }
}
