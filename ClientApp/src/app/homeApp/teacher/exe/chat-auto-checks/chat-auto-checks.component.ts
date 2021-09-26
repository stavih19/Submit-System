import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ChatAdvancedChecksComponent } from './chat-advanced-checks/chat-advanced-checks.component';
import { AutoCheck } from 'src/Modules/AutoChecks/auto-check';
import { TestInput } from 'src/Modules/Teacher/test-input';
import { FileSubmit } from 'src/Modules/file-submit';
import { Test } from 'src/Modules/Teacher/test';

@Component({
  selector: 'app-chat-auto-checks',
  templateUrl: './chat-auto-checks.component.html',
  styleUrls: ['./chat-auto-checks.component.css']
})
export class ChatAutoChecksComponent implements OnInit {
  token: string;
  newTest: TestInput;
  selectedCourse: Course;
  exeID: string;
  lines: AutoCheck[];
  modalRef: any;
  checkoutForm = this.formBuilder.group({
    name: [''],
    input: [''],
    output: [''],
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService,
    public dialog: MatDialog
  ) {
    this.appService.tokenStorage.subscribe(token => this.token = token);
    this.appService.newAutoTestStorage.subscribe(test => this.newTest = test);
   }


  displayedColumns: string[] = ["erase", "weight", "symbol"];

  ngOnInit() {
    //this.newTest = {} as TestInput;
    this.newTest.test = {} as Test;
    this.newTest.test.id = 0;
    this.newTest.test.type = 0;
    this.newTest.test.exerciseID = this.exeID;
    this.newTest.test.weight = 0;
    this.newTest.test.outputFileName = "";
    this.newTest.test.argumentsString = "";
    this.newTest.additionalFiles = [];
    this.getAutoCheckList();
  }

  getAutoCheckList() {
    let url = 'https://localhost:5001/Teacher/ExerciseTests?exerciseId=' + this.exeID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.lines = JSON.parse(data);
        console.log(this.lines);
      }, error => {
        console.log(error);
      }
    );
  }

  async onSubmit() {
    this.newTest.test.input = this.checkoutForm.value.input;
    this.newTest.test.expectedOutput = this.checkoutForm.value.output;

    if(this.newTest.test.exerciseID === null) { return; }

    this.creatTest(this.newTest);
    setTimeout(() => {
      this.getAutoCheckList();
    }, 100);
  }

  creatTest(newTest: TestInput) {
    let paramsToSend = {
      "Test": newTest.test,
      "AdditionalFiles": newTest.additionalFiles
    };
    
    setTimeout(() => {
      console.log(paramsToSend);
      let url = 'https://localhost:5001/Teacher/AddTest';
      this.httpClient.post(url, paramsToSend,
      {responseType: 'text'}).toPromise().then(
        data => {
          console.log(data.toString());
        }, error => {
          console.log(error);
        }
      );
    }, 0);
  }

  async openAdvancedConfig(checkName: string) {
    this.modalRef = this.dialog.open(ChatAdvancedChecksComponent, );
    this.modalRef.componentInstance.selectedCourse = this.selectedCourse;

    this.modalRef.afterClosed().subscribe((_: any) => {
      console.log(this.newTest);
      // this.params.test.outputFileName = this.modalRef.componentInstance.checkoutForm.value.fileRun;
      // this.params.test.argumentsString = this.modalRef.componentInstance.checkoutForm.value.arguments;
      // this.params.test.timeoutInSeconds = this.modalRef.componentInstance.checkoutForm.value.timeout;
      // this.params.test.mainSourseFile = this.modalRef.componentInstance.submitFile.name;
      // this.params.test.adittionalFilesLocation = this.modalRef.componentInstance.checkoutForm.value.desteny;
      // this.params.additionalFiles.push(this.modalRef.componentInstance.submitFile.content);
    });
  }

  getAdvanced(checkName: string) {

  }

  deleteTest(id: number) {
    let url = 'https://localhost:5001/Teacher/DeleteTest?testId=' + id;
    this.httpClient.delete(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data.toString());
        this.getAutoCheckList();
      }, error => {
        console.log(error);
      }
    );
  }
}
