import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {

    @Output() cancelRegister = new EventEmitter();
    user: User;
    registerForm: FormGroup;

    // Datepicker config, partial<> makes every element optional
    bsConfig: Partial<BsDatepickerConfig>;

    constructor(
        private authService: AuthService,
        private alertify: AlertifyService,
        private formBuilder: FormBuilder,
        private router: Router) {}

    ngOnInit() {

        // Datepicker config
        this.bsConfig = {
            containerClass: 'theme-dark-blue'
        };

        // Initialise our reactive form
        this.registerForm = this.formBuilder.group({
            gender: ['male'],
            username: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(4)]],
            confirmPassword: ['', Validators.required],
            firstName: ['', Validators.required],
            insertion: [''],
            lastName: [''],
            dateOfBirth: [null, Validators.required]
        }, {
            validator: this.passwordMatchValidator
        });
    }

    passwordMatchValidator(group: FormGroup) {
        return group.get('password').value === group.get('confirmPassword').value ? null : {mismatch: true};
    }

    register() {

        // Double check the registration is completed
        if (this.registerForm.valid) {
            this.user = Object.assign({}, this.registerForm.value);
        }

        this.authService.register(this.user).subscribe(() => {
            this.alertify.success('registration successful.');
        }, error => {
            this.alertify.error(error);

        // When completed, log the user in.
        }, () => {
            this.authService.login(this.user).subscribe(() => {
                this.router.navigate(['/members']);
            });
        });
    }

    cancel() {
        this.cancelRegister.emit();
    }
}
