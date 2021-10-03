import { HttpClient } from '@angular/common/http';
import { ElementRef, Input } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ApprovalService } from 'src/app/approval.service';
import { Chat } from 'src/Modules/chat';
import { ExtenstionRequest } from 'src/Modules/Extenstion/extenstion-request';
import { Message } from 'src/Modules/message';
import { SubmitDate } from 'src/Modules/Reduce/submit-date';
import { TeacherDateDisplay } from 'src/Modules/Reduce/teacher-date-display';
import { RequestLabel } from 'src/Modules/Teacher/request-label';
import { RequestLabelMainPage } from 'src/Modules/Teacher/request-label-table';

import { ChatDialogComponent } from '../../student/before-submition-exe/chat-dialog/chat-dialog.component';
import { ChatDialogTeacherComponent } from './chat-dialog-teacher/chat-dialog-teacher.component';

@Component({
  selector: 'app-extension',
  templateUrl: './extension.component.html',
  styleUrls: ['./extension.component.css']
})
export class ExtensionComponent implements OnInit {
  //headerText: string = "בקשות הארכה";
  isToShowAlert: boolean = false;
  color: string = "";
  errorMessage: string = "";
  messageList: Message[] = [];
  textMessage: string;
  token: string = "";
  id: string = "";
  modalRef: any;
  chatID: Chat;
  requests: RequestLabel[];
  teams: string[];
  selectedDate: Date;
  reduceNumber: number;
  teacherStatus: string;

  @Input() selectExe: RequestLabelMainPage;
  @Input() headerText: string;
  @ViewChild("alert", {static: false}) alert: ElementRef;
  @ViewChild("approveButton", {static: false}) approveButton: ElementRef;

  reduceTable: TeacherDateDisplay[] = [];
  submitionColumns: string[] = ["teamDate", "teamName"];

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
    public dialog: MatDialog,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
    this.appService.theacherStatusStorage.subscribe(status => this.teacherStatus = status);
  }

  ngOnInit() {
    this.selectedDate = new Date();
    let tommorow = new Date();
    this.selectedDate.setDate(tommorow.getDate() + 1);

    if(this.teacherStatus === "extenstion") {
      this.getExtensions();
    } else if(this.teacherStatus === "appeal"){
      this.getAppeals();
    }
  }

  getExtensions() {
    console.log(this.selectExe);
    let url = 'https://localhost:5001/Teacher/GetExtensions?exerciseId=' + this.selectExe.exerciseID;
    console.log(url);
    this.httpClient.get(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        this.requests = JSON.parse(data);
        console.log(this.requests);
      }, error => {
        this.errorMessagefunc(error.status);
      }
    );
  }

  getAppeals() {
    let url = 'https://localhost:5001/Teacher/GetAppeals?exerciseId=' + this.selectExe.exerciseID;
    console.log(url);
    this.httpClient.get(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        this.requests = JSON.parse(data);
        console.log(this.requests);
      }, error => {
        this.errorMessagefunc(error.status);
      }
    );
  }

  onDeny(request: ExtenstionRequest) {
    const params = {
      msg: this.textMessage
    }

    let url = 'https://localhost:5001/Teacher/RejectRequest?chatId=' + this.selectExe.chatID;
    this.httpClient.post(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        this.getExtensions();
        console.log(data);
      }, error => {
        this.errorMessagefunc(error.status);
      }
    )

    this.alertMessage("Deny message was sent");
  }

  onApprove(request: ExtenstionRequest) {
    const params: SubmitDate = {
      date: this.selectedDate,
      id: 0,
      exerciseID: this.selectExe.exerciseID,
      group: 0,
      reduction: this.reduceNumber,
    }

    console.log(params);

    let url = 'https://localhost:5001/Teacher/AcceptExtension?chatId=' + this.selectExe.chatID;
    this.httpClient.post(url, params,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        this.getExtensions();
      }, error => {
        this.errorMessagefunc(error.status);
      }
    )

    this.alertMessage("Approve message was sent");
  }

  async onReplay(request: ExtenstionRequest) {
    const modalRef =  this.dialog.open(ChatDialogTeacherComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.chatID = this.selectExe.chatID;
  }

  changeIsToShowAlert(isToShowAlert: boolean) {
    this.isToShowAlert = isToShowAlert;
  }

  changeColorAlert(color: string) {
    this.color = color;
    
    this.alert.nativeElement.classList.remove("alert-success");
    this.alert.nativeElement.classList.remove("alert-danger");
    this.alert.nativeElement.classList.remove("alert-warning");
    this.alert.nativeElement.classList.add(color);
  }

  changeMessageAlert(errorMessage: string) {
    console.log(errorMessage);
    this.errorMessage = errorMessage;
  }

  errorMessagefunc(error: string) {
    this.updateShowAlert(true);
    const message = error + "   try again";
    setTimeout(() => {
      this.alert.nativeElement.innerHTML = message;
    }, 0);
    
    setTimeout(() => {
      this.updateShowAlert(false);
    }, 5000);
  }

  alertMessage(message: string) {
    this.updateShowAlert(true);
    setTimeout(() => {
      this.alert.nativeElement.innerHTML = message;
      this.changeColorAlert("alert-success");
    }, 0);
    setTimeout(() => {
      this.updateShowAlert(false);
    }, 5000);
  }

  updateShowAlert(displayFlag: boolean) {
    this.changeIsToShowAlert(displayFlag);
  }

  onDateSelect(id: string) {
    (<HTMLInputElement> document.getElementById(id)).disabled = false;
  }
}
