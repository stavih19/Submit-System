import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';

@Component({
  selector: 'app-chat-advanced-checks',
  templateUrl: './chat-advanced-checks.component.html',
  styleUrls: ['./chat-advanced-checks.component.css']
})
export class ChatAdvancedChecksComponent implements OnInit {
  selectedCourse: Course;
  checkName: string;
  uploadFileList: any[] = [];
  checkoutForm = this.formBuilder.group({
    desteny: [''],
    timeout: [''],
    arguments: [''],
    fileRun: ['']
  });

  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('file1', {static: false}) file1: ElementRef;

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService
  ) { }

  ngOnInit() {
  }

  onSubmit() {
    let params = { 
      "desteny": this.checkoutForm.value.name,
      "timeout": this.checkoutForm.value.input,
      "output": this.checkoutForm.value.output,
      "arguments": this.checkoutForm.value.arguments,
      "extraFiles": this.uploadFileList
    };
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
    //this.isSubmitCheckFunc();
    this.file1.nativeElement.value = "";
  }

  eraseUploadFile(fileToDelete) {
    console.log("earse");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
    //this.isSubmitCheckFunc();
  }
}
