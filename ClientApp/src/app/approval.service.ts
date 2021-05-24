import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ApprovalService {
  private logedIn = new BehaviorSubject(false);
  private userName = new BehaviorSubject(["", ""]);
  private token = new BehaviorSubject("");
  private exeStatus = new BehaviorSubject("");

  logedInStorage = this.logedIn.asObservable();
  userNameStorage = this.userName.asObservable();
  tokenStorage = this.token.asObservable();
  exeStatusStorage = this.exeStatus.asObservable();

  constructor() { }

  updateLoginStatus(status: boolean): void {
    this.logedIn.next(status);
  }

  updateUserName(userName: string[]): void {
    this.userName.next(userName);
  }
  
  updateToken(token: string): void {
    this.token.next(token);
  }

  updateExeStatus(status: string): void {
    this.exeStatus.next(status);
  }
}
