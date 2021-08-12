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
import { ChatDeclarationComponent } from './chat-declaration/chat-declaration.component';
import { async } from '@angular/core/testing';

@Component({
  selector: 'app-before-submition-exe',
  templateUrl: './before-submition-exe.component.html',
  styleUrls: ['./before-submition-exe.component.css']
})
export class BeforeSubmitionExeComponent implements OnInit, AfterContentInit {
  exeStatus: string = "טרם הוגש";
  selectedExe: ExerciseLabel;
  filesMessage: string;
  selectedExeInfo: StudentExInfo;
  uploadFileList: any[] = [];
  modalRef: any;
  isSented: boolean;
  isToCloseModal: boolean = false;
  isSubmitSuccess: boolean = false;
  isSubmitCheck: boolean = true;
  fileContainer: FileSubmit[];
  converstionTarget: string;
  isTrySend: boolean = false;
  checkoutForm = this.formBuilder.group({
    additionalSubmitors: [''],
    uploadFiles: ['']
  });

  token: string;
  detailColummsHeader = ["status", "date", "ids"];
  detailColumns: any[] = [{
    status: "",
    ids: "",
    date: ""
  }];

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
  @ViewChild('file1label', {static: false}) file1label: ElementRef;

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

  fileBefore: string;
  async onSubmit(realSubmit: boolean) {
    console.log("submit");
    this.isSented = false;
    console.log(this.uploadFileList);
    this.fileContainer = [];
    this.uploadFileList.forEach(file => {
      const reader = new FileReader();
      reader.onload = ()=> {
        this.fileBefore = reader.result.toString();
        let submitFile: FileSubmit = {
          content: reader.result.toString(),
          name: file.name
        }
        this.fileContainer.push(submitFile);
      };
      reader.readAsDataURL(file);
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

  async sendSubmission(realSubmit: boolean) {
    const files = [];
    this.fileContainer.forEach(file => {
      files.push({
        "Name": file.name,
        "Content": file.content
      });
    });

    if(files.length === 0) {
      this.errorMessage("No files to send", "alert-danger");
      return;
    }

    const submitters = this.additionalSubmitors.nativeElement.value.split(",");
    if(submitters.length === 0) {
      this.errorMessage("No student id to mach with", "alert-danger");
      return;
    }

    const idResponse = await this.checkSubmittersValidation(submitters);
    if(idResponse !== "") {
      this.errorMessage('The id "' + idResponse + '" ' + "isn't relate to any student", "alert-danger");
      return;
    }

    console.log(files);

    let url = 'https://localhost:5001/Student/SubmitExercise?userid=' + this.token + '&exerciseId=' + this.selectedExe.id;
    this.httpClient.post(url, files,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(files);
        this.errorMessage("Sent successfully", "alert-success");
        this.isSubmitSuccess = true;
        this.exeStatus = "הוגש";
      }, error => {
        //this.errorMessage("Sent successfully", "alert-success");
        //this.isSubmitSuccess = true;
        //this.exeStatus = "הוגש";
        console.log(error);
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }

  isSubmitCheckFunc() {
    if(this.uploadFileList.length === 0 || this.additionalSubmitors.nativeElement.value === "") {
      this.isSubmitCheck = true;
    } else if(this.uploadFileList.length !== 0 && this.additionalSubmitors.nativeElement.value !== ""){
      this.isSubmitCheck = false;
    }
  }

  async onSelect(exe) {
    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    console.log("wait");
    await sleep(100);
    console.log("done");
    this.selectedExe = exe;
    console.log(exe.id);

    let url = 'https://localhost:5001/Student/SubmissionDetails?token=' + this.token + '&exerciseId=' + exe.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
          this.selectedExeInfo = JSON.parse(data);
          this.selectedExeInfo.filenames.forEach(fileName => {
          this.uploadFileList.push(fileName);
          //const file = this.getFilebyName(fileName);
        });
        console.log(this.uploadFileList);
        
        const state = this.selectedExeInfo.state;
        if(state === 0) {
          this.exeStatus = "טרם הוגש";
          this.converstionTarget = "בקש הארכה";
          this.filesMessage = "בחר קבצים";
        } else if(state === 1) {
          this.exeStatus = "הוגש";
          this.converstionTarget = "בקש הארכה";
          this.filesMessage = "בחר קבצים";
          this.isSubmitSuccess = true;
        } else if(state === 2){
          this.exeStatus = "נבדק";
          this.converstionTarget = "הגש ערעור";
          this.filesMessage = "הורד קבצים";
          this.isSubmitSuccess = true;
        } else {
          
        }
        this.fileSubmittersValue();

        this.detailColumns[0].status = this.exeStatus;
        console.log(this.additionalSubmitors.nativeElement.value);
        this.detailColumns[0].ids = this.additionalSubmitors.nativeElement.value;
        this.detailColumns[0].date = this.selectedExeInfo.dates[0].date.toString().substring(0, 10);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }

  getFilebyName(file: string) {
    let submissionID = this.selectedExeInfo.submissionID;
    let url = 'https://localhost:5001/Student/GetFile?userid=' + this.token + '&submissionId=' + submissionID + "&file=" + file;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {     
        data = data.toString();
        this.downloadAttributes(file, data);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );
  }

  downloadFile(file: string) {
    this.getFilebyName(file);
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
    if(this.exeStatus === "נבדק") {
      
    } else {
      const file = event.target.files[0];
      this.uploadFileList.push(file);
      this.isSubmitCheckFunc();
      this.file1.nativeElement.value = "";
    }

    const state = this.selectedExeInfo.state;
    if(state > 0) {
      
    }
  }

  eraseUploadFile(fileToDelete) {
    console.log("earse");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
    this.isSubmitCheckFunc();
  }

  askForChat() {
    this.displayConverstion();
  }

  displayConverstion() { // data: any
    const modalRef =  this.dialog.open(ChatDialogComponent);
    this.modalRef = modalRef;

    const state = this.selectedExeInfo.state;
    if(state === 2) {
      modalRef.componentInstance.chatID = this.selectedExeInfo.extensionChat;
      modalRef.componentInstance.headerMessage = "הגש ערעור";
    } else {
      modalRef.componentInstance.chatID = this.selectedExeInfo.extensionChat;
      modalRef.componentInstance.headerMessage = "בקש הארכה";
    }
    modalRef.componentInstance.teacherName = this.teacherName;
    modalRef.componentInstance.exeName = this.selectExe.name;
  }

  closeModal() {
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
    let submissionID = this.selectedExeInfo.submissionID;
    let url = 'https://localhost:5001/Student/RunResult?token=' + this.token + '&submitId=' + submissionID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        this.displayResults(data.toString());
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );
  }

  displayResults(data: string) {
    const modalRef =  this.dialog.open(ChatDeclarationComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.data = data;
  }

  async checkSubmittersValidation(submitters) {
    let url = 'https://localhost:5001/Student/ValidateSubmitters?userid=' + this.token + '&exerciseId=' + this.selectedExe.id;
    const idRespone = await this.httpClient.post(url, submitters, 
    {responseType: 'text'}).toPromise().then(
      data => {
        const dataObj = JSON.parse(data);
        const dataMap = new Map(Object.entries(dataObj));
        for (let [id, isValid] of dataMap) {
          if(!isValid) { return id; }
        }
        return "";
      }, error => {
        this.errorMessage("Sent successfully", "alert-success");
        return false;
      }
    );

    return idRespone;
  }

  downloadFiles() {
    let extensionChat = this.selectedExeInfo.extensionChat;
    if(extensionChat === null) { return; }
    let url = 'https://localhost:5001/Student/Download?userid=' + this.token + '&submissionId=' + extensionChat.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        this.downloadAttributes("", data.toString());
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );
  }

  downloadAttributes(file: string, data: string) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(data));
    element.setAttribute('download', file);
    element.style.display = "none";
    document.body.appendChild(element);
    element.click();
    element.remove();
  }
}