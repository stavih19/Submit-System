import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild ,DoCheck, KeyValueDiffers, KeyValueDiffer} from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExerciseLabel } from 'src/Modules/ExerciseLabel';
import { FormBuilder } from '@angular/forms';
import { ElementRef } from '@angular/core';
import { SubmissionUpload } from 'src/Modules/SubmissionUpload';
import { StudentExInfo } from 'src/Modules/StudentExInfo';
import { MatDialog } from '@angular/material/dialog';
import { ChatDialogComponent } from './chat-dialog/chat-dialog.component';

@Component({
  selector: 'app-before-submition-exe',
  templateUrl: './before-submition-exe.component.html',
  styleUrls: ['./before-submition-exe.component.css']
})
export class BeforeSubmitionExeComponent implements OnInit {
  selectedExe: ExerciseLabel;
  selectedExeInfo: StudentExInfo;
  uploadFileList: any[] = [];
  modalRef: any;
  isToCloseModal: boolean = false;
  checkoutForm = this.formBuilder.group({
    additionalSubmitors: [''],
    uploadFiles: ['']
  });

  token: string;

  @Input() selectExe: any;
  @Input() teacherName: string;
  @Output() isToShowAlert = new EventEmitter<boolean>();

  updateShowAlert(value: boolean) {
    this.isToShowAlert.emit(value); // ???
  }

  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('fileUploadText', {static: false}) fileUploadText: ElementRef;
  @ViewChild('additionalSubmitors', {static: false}) additionalSubmitors: ElementRef;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    console.log(this.selectExe);
  }

  onSubmit() {
    console.log("submit");

    const inputSubmition: SubmissionUpload = {
      files: this.uploadFileList,
      submitters: this.additionalSubmitors.nativeElement.value
    }

    let url = 'https://localhost:5001/Student/SubmitExercise?token=' + this.token + '&exerciseId=' + this.selectedExe.id;
    this.httpClient.post(url, inputSubmition,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(inputSubmition);
      }, error => {
        this.errorMessage(error.status);
      }
    )
  }

  onSelect(exe) {
    this.selectedExe = exe;
    console.log(exe.id);

    let url = 'https://localhost:5001/Student/SubmissionDetails?token=' + this.token + '&exerciseId=' + exe.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.selectedExeInfo = JSON.parse(data);
        //this.uploadFileList = this.selectedExeInfo. TODO
        this.fileSubmittersValue();
        console.log(this.selectedExeInfo);
      }, error => {
        this.errorMessage(error.status);
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
    const file = event.target.files[0];
    console.log(file.name);

    this.uploadFileList.push(file);
  }

  eraseUploadFile(fileToDelete) {
    console.log("earse");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
  }

  askForExtenstion() {
    let extensionChat = this.selectedExeInfo.extensionChat;
    if(extensionChat === null) { this.displayConverstion([]); return; }
    let url = 'https://localhost:5001/Student/MessageList?token=' + this.token + '&chatId=' + extensionChat.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data);
        this.displayConverstion(data);
      }, error => {
        this.errorMessage(error.status);
      }
    )   
  }

  displayConverstion(data: any) {
    const modalRef =  this.dialog.open(ChatDialogComponent);
    this.modalRef = modalRef;

    modalRef.componentInstance.chatID = this.selectedExeInfo.extensionChat;
    modalRef.componentInstance.teacherName = this.teacherName;
    modalRef.componentInstance.exeName = this.selectExe.name;
  }

  closeModal() {
    //const change = this.isToCloseModal.diff(this);
    this.modalRef.close();
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