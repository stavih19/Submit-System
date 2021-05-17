import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExerciseLabel } from 'src/Modules/ExerciseLabel';
import { FormBuilder } from '@angular/forms';
import { ElementRef } from '@angular/core';
import { SubmissionUpload } from 'src/Modules/SubmissionUpload';

@Component({
  selector: 'app-before-submition-exe',
  templateUrl: './before-submition-exe.component.html',
  styleUrls: ['./before-submition-exe.component.css']
})
export class BeforeSubmitionExeComponent implements OnInit {
  isToShowAlert: boolean = false;
  exeList: ExerciseLabel[];
  selectedExe: ExerciseLabel;
  selectedExeInfo: any;
  uploadFileList: any[] = [];
  checkoutForm = this.formBuilder.group({
    additionalSubmitors: [''],
    uploadFiles: ['']
  });

  token: string;

  @Input() selectExe: any;
  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('fileUploadText', {static: false}) fileUploadText: ElementRef;
  @ViewChild('additionalSubmitors', {static: false}) additionalSubmitors: ElementRef;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
    private formBuilder: FormBuilder,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    console.log(this.selectExe);
    this.getExeLists(this.selectExe.courseID);
    setTimeout(() => {
      this.onSelect(this.exeList[0]); // TODO
    }, 0);

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
        console.log(error);

        this.isToShowAlert = true;
        setTimeout(() => {
          const message = "קוד שגיאה:&nbsp;" + error.status + "&nbsp;&nbsp;נסה מאוחר יותר";
          document.getElementById("alertEle").innerHTML = message;
        }, 0);
        setTimeout(() => {
          this.isToShowAlert = false;
        }, 5000);
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
        console.log(error);

        this.isToShowAlert = true;
        const message = error.status + "   try again";
        document.getElementById("alertEle").innerHTML = message;
        setTimeout(() => {
          this.isToShowAlert = false;
        }, 5000);
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

  getExeLists(courseID) {
    let url = 'https://localhost:5001/Student/ExerciseLabels?token=' + this.token + '&coursed=' + courseID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeList = JSON.parse(data);
      }, error => {
        console.log(error);

        this.exeList = [
          {id: "123456789", name: "ex 3"},
          {id: "123456789", name: "ex 2"},
          {id: "123456789", name: "ex 1"},
        ];

        this.isToShowAlert = true;
        const message = error.status + "   try again";
        document.getElementById("alertEle").innerHTML = message;
        setTimeout(() => {
          this.isToShowAlert = false;
        }, 5000);
      }
    )
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
}
