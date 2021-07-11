import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild ,DoCheck, KeyValueDiffers, KeyValueDiffer, AfterContentInit} from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExerciseLabel } from 'src/Modules/exercise-label';
import { FormBuilder } from '@angular/forms';
import { ElementRef } from '@angular/core';
import { StudentExInfo } from 'src/Modules/student-exInfo';
import { MatDialog } from '@angular/material/dialog';
import { FileSubmit } from 'src/Modules/file-submit';
import { ChatDialogComponent } from './chat-dialog/chat-dialog.component';

@Component({
  selector: 'app-before-submition-exe',
  templateUrl: './before-submition-exe.component.html',
  styleUrls: ['./before-submition-exe.component.css']
})
export class BeforeSubmitionExeComponent implements OnInit, AfterContentInit {
  exeStatus: string = "טרם הוגש";
  selectedExe: ExerciseLabel;
  selectedExeInfo: StudentExInfo;
  uploadFileList: any[] = [];
  modalRef: any;
  isSented: boolean;
  isToCloseModal: boolean = false;
  isSubmitSuccess: boolean = false;
  isSubmitCheck: boolean = false;
  fileContainer: FileSubmit[];
  converstionTarget: string;
  checkoutForm = this.formBuilder.group({
    additionalSubmitors: [''],
    uploadFiles: ['']
  });

  token: string;

  @Input() selectExe: any;
  @Input() teacherName: string;
  @Output() isToShowAlert = new EventEmitter<boolean>();
  @Output() color = new EventEmitter<string>();
  @Output() errorMessageText = new EventEmitter<string>();

  updateShowAlert(value: boolean, color: string, message: string) {
    if(value) {
      this.isToShowAlert.emit(value);
    }
    setTimeout(() => {
      this.color.emit(color);
      this.errorMessageText.emit(message);
      this.isToShowAlert.emit(value);
    }, 0);
  }

  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('fileUploadText', {static: false}) fileUploadText: ElementRef;
  @ViewChild('additionalSubmitors', {static: false}) additionalSubmitors: ElementRef;
  @ViewChild('file1', {static: false}) file1: ElementRef;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }
  ngAfterContentInit(): void {
    setTimeout(() => {
      this.additionalSubmitors.nativeElement.addEventListener('input', this.isSubmitCheckFunc);
    }, 0);
  }

  ngOnInit() {
    console.log(this.selectExe);
  }

  async onSubmit(realSubmit: boolean) {
    console.log("submit");
    this.isSented = false;

    console.log(this.uploadFileList);
    this.fileContainer = [];
    this.uploadFileList.forEach(file => {
      const reader = new FileReader();
      reader.onload = ()=> {
        let submitFile: FileSubmit = {
          content: btoa(reader.result.toString()),
          name: file.name
        }
        this.fileContainer.push(submitFile);
      };
      reader.readAsBinaryString(file);
    });

    setTimeout(() => {
      this.isSented = true;
      setTimeout(() => {
        this.sendSubmission(realSubmit);
      }, 2000);
    }, 3000);

    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    while(!this.isSented) {
      this.guiMessages("Sending Files", "alert-warning");
      await sleep(500);
      this.guiMessages("Sending Files.", "alert-warning"); 
      await sleep(500);
      this.guiMessages("Sending Files..", "alert-warning"); 
      await sleep(500);
      this.guiMessages("Sending Files...", "alert-warning"); 
      await sleep(500);
    }
  }

  sendSubmission(realSubmit: boolean) {
    const inputSubmition = {
      "RealSubmit": realSubmit,
      "Files": this.fileContainer,
      "Submitters": this.additionalSubmitors.nativeElement.value.split(",")
    }

    if(inputSubmition.Files.length === 0) {
      this.errorMessage("No files to send", "alert-danger");
      return;
    }
    if(inputSubmition.Submitters.length === 0) {
      this.errorMessage("No student id to mach with", "alert-danger");
      return;
    }

    let url = 'https://localhost:5001/Student/SubmitExercise?token=' + this.token + '&exerciseId=' + this.selectedExe.id;
    this.httpClient.post(url, inputSubmition,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(inputSubmition);
        this.errorMessage("Sent successfully", "alert-success");
        this.isSubmitSuccess = true;
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }

  isSubmitCheckFunc() {
    if(this.uploadFileList.length === 0 || this.additionalSubmitors.nativeElement.value === "") {
      this.isSubmitCheck = false;
    } else if(this.uploadFileList.length !== 0 && this.additionalSubmitors.nativeElement.value !== ""){
      this.isSubmitCheck = true;
    }
  }

  onSelect(exe) {
    this.selectedExe = exe;
    console.log(exe.id);

    let url = 'https://localhost:5001/Student/SubmissionDetails?token=' + this.token + '&exerciseId=' + exe.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.selectedExeInfo = JSON.parse(data);
        //this.uploadFileList = this.selectedExeInfo.files;
        if(this.uploadFileList.length === 0) {
          this.exeStatus = "טרם הוגש";
          this.converstionTarget = "בקש הארכה";
        } else if(this.selectedExeInfo.dates[0].date > new Date()) {
          this.exeStatus = "הוגש";
          this.converstionTarget = "בקש הארכה";
        } else {
          this.exeStatus = "ממתתין לבדיקה";
          this.converstionTarget = "הגש ערעור";
          this.isSubmitSuccess = true;
        }
        this.fileSubmittersValue();
        console.log(this.selectedExeInfo);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }

  fileSubmittersValue() {
    this.additionalSubmitors.nativeElement.value = "";
    this.selectedExeInfo.submitters.forEach(submitter => {
      if(this.additionalSubmitors.nativeElement.value !== "") {
        this.additionalSubmitors.nativeElement.value += "," + submitter.id;
      } else {
        this.additionalSubmitors.nativeElement.value += submitter.id;
      }
    });
  }

  onDragOver(event) {
    this.fileUploadBox.nativeElement.style.borderStyle = "solid";
  }

  onDragLeave(event) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
  }

  onDrop(event) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
    console.log("drop");
    /*let file = event.dataTransfer.files[0];

    let fileEle = document.createElement("span");
    fileEle.id = file.name;
    fileEle.innerHTML = file.name;

    this.fileUploadText.nativeElement.appendChild(fileEle);*/
  }

  onFileChoose(event) {
    console.log(event.target.files[0].name);
    const file = event.target.files[0];
    this.uploadFileList.push(file);
    this.isSubmitCheckFunc();
    this.file1.nativeElement.value = "";
  }

  eraseUploadFile(fileToDelete) {
    console.log("earse");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
    this.isSubmitCheckFunc();
  }

  askForExtenstion() {
    this.displayConverstion();
    /*let extensionChat = this.selectedExeInfo.extensionChat;
    if(extensionChat === null) { this.displayConverstion([]); return; }
    let url = 'https://localhost:5001/Student/MessageList?token=' + this.token + '&chatId=' + extensionChat.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data);
        this.displayConverstion(data);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )*/
  }

  displayConverstion() { // data: any
    const modalRef =  this.dialog.open(ChatDialogComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.chatID = this.selectedExeInfo.extensionChat;
    modalRef.componentInstance.teacherName = this.teacherName;
    modalRef.componentInstance.exeName = this.selectExe.name;
    //modalRef.componentInstance.messageList = data;
  }

  closeModal() {
    //const change = this.isToCloseModal.diff(this);
    this.modalRef.close();
  }

  errorMessage(message: string, color:string) {
    this.updateShowAlert(true, color, message);
    setTimeout(() => {
      this.updateShowAlert(false, color, message);
    }, 5000);
  }

  guiMessages(message: string, color:string) {
    this.updateShowAlert(true, color, message);
  }

  lastFileRun() {
    let extensionChat = this.selectedExeInfo.extensionChat;
    if(extensionChat === null) { return; }
    let url = 'https://localhost:5001/Student/RunResult?token=' + this.token + '&submitId=' + extensionChat.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data);
        console.log(data);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }
}