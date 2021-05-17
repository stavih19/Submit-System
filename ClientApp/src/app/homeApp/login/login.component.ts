import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { HomeComponentComponent } from 'src/app/homeApp/home-component/home-component.component';
import { ApprovalService } from "src/app/approval.service";
import { error } from 'selenium-webdriver';

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
    let params = { 
      "Username": this.checkoutForm.value.userName,
      "Password": this.checkoutForm.value.password,
    };
    console.log(params);

    this.httpClient.post('https://localhost:5001/User/Login', params, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);

        this.appService.updateLoginStatus(true);
        this.appService.updateUserName(this.checkoutForm.value.userName);
        this.appService.updateToken(data);

        this.checkoutForm.reset();
      }, error => {
        console.log(error);
      }
    )
  }
}