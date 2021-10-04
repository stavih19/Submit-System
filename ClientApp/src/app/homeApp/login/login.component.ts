import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { HomeComponentComponent } from 'src/app/homeApp/home-component/home-component.component';
import { ApprovalService } from "src/app/approval.service";
import { error } from 'selenium-webdriver';
import { TestInput } from 'src/Modules/Teacher/test-input';
import { DataInput } from 'src/Modules/data-input';

const hour: number = 3600000;
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  checkoutForm = this.formBuilder.group({
    userName: ['576888433'],
    password: ['password']
  });
  static logedIn: any;
  
  constructor(
    private formBuilder: FormBuilder,
    private httpClient: HttpClient,
    private appService: ApprovalService,
  ) {
    
  }

  ngOnInit() {}

  onSubmit(): void {    
    let params = { 
      "Username": this.checkoutForm.value.userName,
      "Password": this.checkoutForm.value.password,
    };


    this.httpClient.post('https://localhost:5001/User/Login', params,
    {responseType: 'text'}).toPromise().then(
      data => {
        let dataInput: DataInput = JSON.parse(data);
        console.log(dataInput);

        this.appService.updateLoginStatus(true);
        if(dataInput.isAdmin === false) {
          this.appService.updateUserName(["false", dataInput.name]);
        } else if(dataInput.isAdmin === true) {
          this.appService.updateUserName(["true", dataInput.name]);
        }
        this.appService.updateToken(dataInput.name);

        this.checkoutForm.reset();
        setTimeout(() => {
          this.turnOff();
        }, hour);
      }, error => {
        console.log(error);
      }
    );
  }

  turnOff() {
    this.appService.updateLoginStatus(false);

    this.appService.updateUserName(["", ""]);
    this.appService.updateToken("");
    this.appService.updateExeStatus("");
    this.appService.updateTheacherStatus("");
    this.appService.updateNewAutoStatus({ } as TestInput);
  }
}