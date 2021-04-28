import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { HomeComponentComponent } from 'src/app/homeApp/home-component/home-component.component';
import { ApprovalService } from "src/app/approval.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  checkoutForm = this.formBuilder.group({
    userName: [''],
    password: ['']
  });
  static logedIn: any;
  
  constructor(
    private formBuilder: FormBuilder,
    private httpClient: HttpClient,
    private appService: ApprovalService,
  ) { }

  ngOnInit() {}

  onSubmit(): void {
    console.warn('Your order has been submitted', this.checkoutForm.value);
    
    let params = new HttpParams();
    params = params.append('user', this.checkoutForm.value.userName);
    params = params.append('password', this.checkoutForm.value.password);

    //let response = this.httpClient.get('http://localhost:3000/Home/login', {params: params}); // TODO
    this.appService.updateLoginStatus(true);
    this.appService.updateUserName(this.checkoutForm.value.userName);

    this.checkoutForm.reset();
  }
}