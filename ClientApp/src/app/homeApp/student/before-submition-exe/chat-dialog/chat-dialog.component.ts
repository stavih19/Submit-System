import { HttpClient } from '@angular/common/http';
import { AfterContentInit, ElementRef, EventEmitter, Output } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { Chat } from 'src/Modules/Chat';
import { Message } from 'src/Modules/Message';
import { BeforeSubmitionExeComponent } from '../before-submition-exe.component';

@Component({
  selector: 'app-chat-dialog',
  templateUrl: './chat-dialog.component.html',
  styleUrls: ['./chat-dialog.component.css']
})
export class ChatDialogComponent implements OnInit, AfterContentInit {
  chatID: Chat;
  teacherName: string;
  exeName: string;
  isThereMessage: boolean = false;
  textMessage: string;
  fileList: any[] = [];
  messageList: Message[];
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
    this.fillConverstionMessages();
  }

  enableButton() {
    this.textMessage = this.textMessageRef.nativeElement.value;
    if(this.textMessage !== "" || this.fileList.length) {
      this.isThereMessage = true;
      console.log(this.isThereMessage);
    } else {
      this.isThereMessage = false;
      console.log(this.isThereMessage);
    }
  }

  fillConverstionMessages() {
    let url = 'https://localhost:5001/Student/MessageList?token=' + this.token + '&chatId=' + this.chatID
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
    if(confirm("Send this message")) {
      this.updateCloseModal(true);
    }
  }

  errorMessage(error: string) {
    this.updateShowAlert(true);
    const message = error + "   try again";
    document.getElementById("alertEle").innerHTML = message;
    setTimeout(() => {
      this.updateShowAlert(false);
    }, 5000);
  }
}
