import { HttpClient } from '@angular/common/http';
import { ElementRef, Input } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ApprovalService } from 'src/app/approval.service';
import { Chat } from 'src/Modules/chat';
import { ExtenstionRequest } from 'src/Modules/Extenstion/extenstion-request';
import { Message } from 'src/Modules/message';
import { TeamLabel } from 'src/Modules/Reduce/team-label';
import { ChatDialogComponent } from '../../student/before-submition-exe/chat-dialog/chat-dialog.component';

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

  @Input() selectExe: any;
  @Input() headerText: string;
  @ViewChild("alert", {static: false}) alert: ElementRef;
  @ViewChild("approveButton", {static: false}) approveButton: ElementRef;

  requests: ExtenstionRequest[] = [
    {
      name: "יוסי",
      id: 123456789,
      text: "שלום מתרגל",
      file: "file.pdf",
      team: "01"
    },
    {
      name: "רמי",
      id: 123456789,
      text: "שלום מתרגל יקר",
      file: "file.pdf",
      team: "03"
    }
  ];

  teams: string[] = ["01", "02", "03"];
  selectedOption: string = "01";

  reduceTable: TeamLabel[] = [
    {
      name: "01",
      date: new Date()
    }, {
      name: "02",
      date: new Date()
    }, {
      name: "03",
      date: new Date()
    }
  ];
  submitionColumns: string[] = ["teamDate", "teamName"];
  preTeamSelect: string = this.selectedOption;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
    public dialog: MatDialog,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() { }

  onDeny(request: ExtenstionRequest) {
    const params = {
      msg: this.textMessage
    }

    /*let url = 'https://localhost:5001/Student/NewMessage?userid=' + this.token + '&chatId=' + this.chatID.id;
    this.httpClient.post(url, params,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        const message = {
          id: "",
          senderID: "",
          date: new Date(),
          body: this.textMessage,
          isTeacher: false
        }
        this.messageList.push(message);
      }, error => {
        this.errorMessagefunc(error.status);
      }
    )*/

    this.alertMessage("Deny message was sent");
  }

  onApprove(request: ExtenstionRequest) {
    const params = {
      msg: this.textMessage
    }

    /*let url = 'https://localhost:5001/Student/NewMessage?userid=' + this.token + '&chatId=' + this.chatID.id;
    this.httpClient.post(url, params,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        const message = {
          id: "",
          senderID: "",
          date: new Date(),
          body: this.textMessage,
          isTeacher: false
        }
        this.messageList.push(message);
      }, error => {
        this.errorMessagefunc(error.status);
      }
    )*/

    this.alertMessage("Approve message was sent");
  }

  async onReplay(request: ExtenstionRequest) {
    this.getChatID(this.selectExe.exID);
    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    await sleep(100);
    const modalRef =  this.dialog.open(ChatDialogComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.chatID = this.chatID;
    modalRef.componentInstance.teacherName = request.name;
    modalRef.componentInstance.exeName = this.selectExe.name;
  }

  teamSelected(request: ExtenstionRequest) {
    console.log(request);
    console.log(this.selectedOption);
    let approveButton = document.getElementById(request.id.toString()) as HTMLButtonElement;
    if(request.team !== this.selectedOption) {
      approveButton.disabled = false;
    } else {
      approveButton.disabled = true;
    }
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

  getChatID(exeId: string) {
    let url = 'https://localhost:5001/Student/SubmissionDetails?userid=' + this.token + '&exerciseId=' + exeId;
    this.httpClient.get(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        let submissionDetails = JSON.parse(data);
        this.chatID = submissionDetails.extensionChat;
      }, error => {
        this.errorMessagefunc(error.status);
      }
    )
  }
}
