import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild ,DoCheck, KeyValueDiffers, KeyValueDiffer, AfterContentInit} from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExerciseLabel } from 'src/Modules/exercise-label';
import { FormBuilder } from '@angular/forms';
import { ElementRef } from '@angular/core';
import { StudentExInfo } from 'src/Modules/Student/student-exInfo';
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
      console.log(this.selectExe);
      this.onSelect(this.selectExe);
    }, 0);
  }

  ngOnInit() {
    
  }

  fileBefore: string;
  async onSubmit(realSubmit: boolean) {
    console.log("submit");
    this.isSented = false;
    console.log(this.uploadFileList);
    this.fileContainer = [];
    this.uploadFileList.forEach(file => {
      console.log(file);
      const reader = new FileReader();
      reader.onload = ()=> {
        this.fileBefore = reader.result.toString();
        let submitFile: FileSubmit = {
          content: reader.result.toString().split(',')[1],
          name: file.name
        }
        this.fileContainer.push(submitFile);
      };
      reader.readAsDataURL(file);
    });

    console.log(this.selectedExe.id);

    setTimeout(() => {
      this.isSented = true;
      setTimeout(() => {
        this.sendSubmission(realSubmit);
      }, 2000);
    }, 3000);

    const sleep = (delay: number) => new Promise((resolve) => setTimeout(resolve, delay));
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
    console.log(files);

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

    let url = 'https://localhost:5001/Student/SubmitExercise?exerciseId=' + this.selectedExe.id + '&final=' + realSubmit;
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
    console.log(this.uploadFileList.length === 0);
    console.log(this.additionalSubmitors.nativeElement.value === "");
    if(this.uploadFileList.length === 0 || this.additionalSubmitors.nativeElement.value === "") {
      this.isSubmitCheck = true;
    } else if(this.uploadFileList.length !== 0 && this.additionalSubmitors.nativeElement.value !== ""){
      this.isSubmitCheck = false;
    }
  }

  async onSelect(exe: any) {
    const sleep = (delay: number) => new Promise((resolve) => setTimeout(resolve, delay));
    console.log("wait");
    await sleep(100);
    console.log("done");
    this.selectedExe = exe;
    console.log(this.selectedExe);
    console.log(exe.id);

    let id = exe.id;
    if(id === undefined) { id = exe.exID; }
    let url = 'https://localhost:5001/Student/SubmissionDetails?exerciseId=' + id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      async data => {
        this.selectedExeInfo = JSON.parse(data);
        this.selectedExeInfo.filenames.forEach(fileName => {
          this.getFilebyName(fileName, false);
        });
        await sleep(1000);
        console.log(this.uploadFileList);
        
        const state = this.selectedExeInfo.state;
        console.log(state);
        if(state === 0) {
          this.exeStatus = "טרם הוגש";
          this.converstionTarget = "בקש הארכה";
          this.filesMessage = "בחר קבצים";
        } else if(state === 1) {
          this.exeStatus = "הוגש";
          this.converstionTarget = "בקש הארכה";
          this.filesMessage = "בחר קבצים";
          this.isSubmitSuccess = true;
        } else if(state === 2) {
          this.exeStatus = "נבדק";
          this.converstionTarget = "הגש ערעור";
          this.filesMessage = "הורד קבצים";
          this.isSubmitSuccess = true;
        } else {
          
        }
        this.fileSubmittersValue();
        this.isSubmitCheckFunc();

        this.detailColumns[0].status = this.exeStatus;
        console.log(this.additionalSubmitors.nativeElement.value);
        this.detailColumns[0].ids = this.additionalSubmitors.nativeElement.value;
        this.detailColumns[0].date = this.selectedExeInfo.dates[0].date.toString().substring(0, 10);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    )
  }

  getFilebyName(file: string, toDownload: boolean) {
    let submissionID = this.selectedExeInfo.submissionID;
    let url = 'https://localhost:5001/Student/GetFile?userid=' + this.token + '&submissionId=' + submissionID + "&file=" + file;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {     
        let dataStr = data.toString();
        if(toDownload) {
          this.downloadAttributes(file, dataStr);
        } else {
          console.log(dataStr);
          var blob = new Blob([dataStr], { type: 'text/plain' });
          var fileObj = new File([blob], file, {type: "text/plain"});
          console.log(fileObj);
          this.uploadFileList.push(fileObj);
        }
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );
  }

  downloadFile(file: string) {
    this.getFilebyName(file, true);
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

  onDragOver(event: any) {
    this.fileUploadBox.nativeElement.style.borderStyle = "solid";
  }

  onDragLeave(event: { preventDefault: () => void; }) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
  }

  onDrop(event: { preventDefault: () => void; }) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
    console.log("drop");
    /*let file = event.dataTransfer.files[0];

    let fileEle = document.createElement("span");
    fileEle.id = file.name;
    fileEle.innerHTML = file.name;

    this.fileUploadText.nativeElement.appendChild(fileEle);*/
  }

  onFileChoose(event: { target: { files: any[]; }; }) {
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

  eraseUploadFile(fileToDelete: any) {
    if(this.exeStatus === "נבדק") { return; }
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
      modalRef.componentInstance.headerMessage = "הגש ערעור";
      modalRef.componentInstance.chatID = this.selectedExeInfo.appealChat;
    } else {
      modalRef.componentInstance.headerMessage = "בקש הארכה";
      modalRef.componentInstance.chatID = this.selectedExeInfo.extensionChat;
    }
    console.log(this.selectedExeInfo);
    modalRef.componentInstance.selectedExeInfo = this.selectedExeInfo;
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
    let url = 'https://localhost:5001/Student/RunResult?submitId=' + submissionID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(JSON.parse(data)); 
        this.displayResults(JSON.parse(data)["text"]);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );
    let a = {
      "text": "Coding Style Test: Grade: 100",
      "autoGrade": -1,
      "styleGrade": 100
    };
  }

  displayResults(data: string) {
    //data = '{"text":"\nCoding Style Test:\n\nGrade: 100\n","autoGrade":-1,"styleGrade":100}';
    const modalRef =  this.dialog.open(ChatDeclarationComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.data = data;
  }

  async checkSubmittersValidation(submitters: any) {
    let url = 'https://localhost:5001/Student/ValidateSubmitters?exerciseId=' + this.selectedExe.id;
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
    let url = 'https://localhost:5001/Student/Download?submissionId=' + this.selectedExeInfo.submissionID;
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