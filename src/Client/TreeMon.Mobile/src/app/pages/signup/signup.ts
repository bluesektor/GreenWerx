import { Component, ViewEncapsulation } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { User } from '../../models';
import { UserService} from '../../services/user/user.service';
// import { UserOptions } from '../../interfaces/user-options';
import { ServiceResult } from '../../models/index';
import { Events } from '@ionic/angular';

@Component({
  selector: 'page-signup',
  templateUrl: 'signup.html',
  styleUrls: ['./signup.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SignupPage {
  // signup: UserOptions = { username: '', password: '' };
  submitted = false;
  private user: User = new User();
  signupErrorString: string;
  loading = false;
  message = '';
  constructor(
    public router: Router,
    private userService: UserService,
    public messages: Events
  ) {
    this.user.ConfirmPassword = '';
  }

  onSignup(form: NgForm ) {
    this.message = '';
    console.log('SIGNTUP.TS onSignup form', form);
    this.submitted = true;
    this.loading = true;

    if ( form.valid === false) {
      console.log('signup.ts invalid form');
      return;
    }
    if ( this.validateForm() === false) {
      console.log('signup.ts validateForm false');
      return;
    }

    this.userService.register(this.user).subscribe((response) => {
      const data = response as ServiceResult;
      this.loading =  false;
      console.log('SIGNTUP.TS onSignup response', data);
      if (data.Code !== 200) {
       // this.messages.publish('api:err', data);
       this.message =  data.Message;
        return false;
      }
      this.message =  data.Message + ' <br/>Registration successful,, you will be redirected to the login page.';
     // this.messages.publish('api:ok', 'Registration successful, you will be redirected to the login page.');

      this.user = new User();
      setTimeout(() => {
          this.router.navigateByUrl('/login');
         },  5000);

     }, (err) => {
      this.loading =  false;
      this.messages.publish('service:err', err);
     });

   }

   validateForm(): boolean {
     this.signupErrorString = '';

     if (this.user.Password !== this.user.ConfirmPassword) {
       this.signupErrorString = 'Passwords must match.';
       return false;
     }

     return true;
   }
}
