import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { FileSubmit } from 'src/Modules/file-submit';
import { TestInput } from 'src/Modules/Teacher/test-input';
import { ChatAutoChecksComponent } from '../chat-auto-checks.component';

@Component({
  selector: 'app-chat-advanced-checks',
  templateUrl: './chat-advanced-checks.component.html',
  styleUrls: ['./chat-advanced-checks.component.css']
})
export class ChatAdvancedChecksComponent implements OnInit {
  selectedCourse: Course;
  newTest: TestInput;
  checkName: string;
  uploadFileList: any[] = [];
  submitFile: FileSubmit;
  anyInput: any;
  checkoutForm = this.formBuilder.group({
    desteny: [''],
    timeout: [0],
    arguments: [''],
    fileRun: ['']
  });

  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('file1', {static: false}) file1: ElementRef;

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService,
    private dialogRef: MatDialogRef<ChatAdvancedChecksComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.appService.newAutoTestStorage.subscribe(test => this.newTest = test);
   }

  ngOnInit() {
    this.submitFile = {} as FileSubmit;
    this.submitFile.name = "";
    this.submitFile.content = "";
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

  onFileSelect(event) {
    console.log(event.target.files[0].name);
    const file = event.target.files[0];

    const reader = new FileReader();
    reader.onload = ()=> {
      this.submitFile.content = reader.result.toString();
      this.submitFile.name = file.name;-
      this.newTest.additionalFiles.push(this.submitFile);
    };
    console.log("Start Reading");
    reader.readAsDataURL(file);
  }

  eraseUploadFile(fileToDelete) {
    console.log("erase");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
    //this.isSubmitCheckFunc();
  }

  onSubmit() {
    this.newTest.test.outputFileName = this.checkoutForm.value.desteny;
    this.newTest.test.timeoutInSeconds = parseInt(this.checkoutForm.value.timeout);
    this.newTest.test.argumentsString = this.checkoutForm.value.arguments;
    this.newTest.test.mainSourseFile = this.checkoutForm.value.fileRun;

    this.dialogRef.close();
  }
}
