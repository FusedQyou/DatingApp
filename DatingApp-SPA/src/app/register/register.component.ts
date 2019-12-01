import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {

    @Output() cancelRegister = new EventEmitter();
    model: any = {};
    registerForm: FormGroup;

    // Datepicker config, partial<> makes every element optional
    bsConfig: Partial<BsDatepickerConfig>;

    constructor(
        private authService: AuthService,
        private alertify: AlertifyService,
        private formBuilder: FormBuilder) {}

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
