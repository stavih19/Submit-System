import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Test } from 'src/Modules/Teacher/test';
import { TestInput } from 'src/Modules/Teacher/test-input';

@Injectable({
  providedIn: 'root'
})

export class ApprovalService {
  private logedIn = new BehaviorSubject(false);
  private userName = new BehaviorSubject(["", ""]);
  private token = new BehaviorSubject("");
  private exeStatus = new BehaviorSubject("");
  private theacherStatus = new BehaviorSubject("");
  private newAutoTest = new BehaviorSubject({ } as TestInput);

  logedInStorage = this.logedIn.asObservable();
  userNameStorage = this.userName.asObservable();
  tokenStorage = this.token.asObservable();
  exeStatusStorage = this.exeStatus.asObservable();
  theacherStatusStorage = this.theacherStatus.asObservable();
  newAutoTestStorage = this.newAutoTest.asObservable();

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

  updateTheacherStatus(status: string): void {
    this.theacherStatus.next(status);
  }

  updateNewAutoStatus(test: TestInput): void {
    this.newAutoTest.next(test);
  }
}