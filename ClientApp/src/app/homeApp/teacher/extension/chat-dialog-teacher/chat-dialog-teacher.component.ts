import { HttpClient } from '@angular/common/http';
import { AfterContentInit, ElementRef, EventEmitter, Output } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { Chat } from 'src/Modules/chat';
import { Message } from 'src/Modules/message';
import { MessageInput } from 'src/Modules/Student/message-input';
import { StudentExInfo } from 'src/Modules/Student/student-exInfo';
import { SubmitFile } from 'src/Modules/Teacher/submit-file';

@Component({
  selector: 'app-chat-dialog-teacher',
  templateUrl: './chat-dialog-teacher.component.html',
  styleUrls: ['./chat-dialog-teacher.component.css']
})
export class ChatDialogTeacherComponent implements OnInit {
  chatID: Chat;
  selectedExeInfo: StudentExInfo;
  headerMessage: string;
  teacherName: string;
  exeName: string;
  isThereMessage: boolean = false;
  textMessage: string;
  fileList: any[] = [];
  messageList: Message[] = [];
  token: string;
  id: string;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
  ) {
    this.appService.tokenStorage.subscribe(token => this.token = token);
    this.appService.tokenStorage.subscribe(id => this.id = id);
   }

  @ViewChild('textMessage', {static: false}) textMessageRef: ElementRef;
  @ViewChild('mesasgeConverstion', {static: false}) mesasgeConverstion: ElementRef;
  @Output() isToCloseModal = new EventEmitter<boolean>();
  @Output() closeModal = new EventEmitter<boolean>();

  updateShowAlert(value: boolean) {
    this.isToCloseModal.emit(value);
  }

  updateCloseModal(value: boolean) {
    this.closeModal.emit(value);
  }

  ngOnInit() {

  }

  ngAfterContentInit(): void {
    setTimeout(() => {
      this.fillConverstionMessages();
    }, 0);
  }

  enableButton() {
    this.textMessage = this.textMessageRef.nativeElement.value;
    if(this.textMessage !== "" || this.fileList.length) {
      this.isThereMessage = true;
    } else {
      this.isThereMessage = false;
    }
  }

  fillConverstionMessages() {
    if(this.chatID == null) {
      this.messageList = [];
      return;
    }

    let url = 'https://localhost:5001/Student/MessageList?chatId=' + this.chatID.id
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.messageList = JSON.parse(data);
        console.log(this.messageList);
      }, error => {
        this.errorMessage(error.status);
      }
    )
  }

  sendMessage() {
    if(confirm("Send this message?")) {
      this.updateCloseModal(true);

      const params = {
        chatID: "",
        text: this.textMessage,
        attachedFile: null
      }

      if(this.chatID == null) {
        let url = 'https://localhost:5001/Student/ExtensionRequest?submissionid=' + this.selectedExeInfo.submissionID;
        this.httpClient.post(url, params,
        {responseType: 'text'}).toPromise().then(
          data => {
            console.log(data);
            this.fillConverstionMessages();
            this.textMessageRef.nativeElement.value = "";

            this.chatID = JSON.parse(data);
          }, error => {
            this.errorMessage(error.status);
          }
        );
      } else {
        params.chatID = this.chatID.id;

        let url = 'https://localhost:5001/Student/NewMessage?chatId=' + this.chatID.id;
        this.httpClient.post(url, params,
        {responseType: 'text'}).toPromise().then(
          data => {
            console.log(data);
            const message: Message = {
              id: "",
              senderID: "",
              date: "Just now",
              body: this.textMessage,
              isTeacher: false
            }
            this.messageList.push(message);
            this.textMessageRef.nativeElement.value = "";
          }, error => {
            this.errorMessage(error.status);
          }
        );
      }
    }
  }

  errorMessage(error: string) {
    this.updateShowAlert(true);
    const message = error + "   try again";
    //document.getElementById("alertEle").innerHTML = message;
    setTimeout(() => {
      this.updateShowAlert(false);
    }, 5000);
  }
}