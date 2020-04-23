import { Router } from '@angular/router';
import { User } from './../_models/user';
import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  user: User;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  // Partial makes all of the propertise optional
  bsConfig: Partial<BsDatepickerConfig>;
  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red',
    };
    this.createRegisterForm();
    // this.registerForm = new FormGroup(
    //   {
    //     username: new FormControl('', Validators.required),
    //     password: new FormControl('', [
    //       Validators.required,
    //       Validators.minLength(4),
    //       Validators.maxLength(8),
    //     ]),
    //     confirmPassword: new FormControl('', Validators.required),
    //   },
    //   this.passwordMatchValidator
    // );             phone: [null, Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(10)])]

  }
  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '', Validators.compose([
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(8)])
        ],
        confirmPassword: ['', Validators.required]

      }, { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      // to get the values from out of registerForm
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => {
          this.alertify.success('Registration Successful!');
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('canceled');
  }
}
