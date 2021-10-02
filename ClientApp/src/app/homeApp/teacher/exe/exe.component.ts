import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { Course } from 'src/Modules/course';
import { ChatReduceComponent } from './chat-reduce/chat-reduce.component';
import { ChatRecudeTeamsComponent } from './chat-recude-teams/chat-recude-teams.component';
import { ChatEditTeacherComponent } from './chat-edit-teacher/chat-edit-teacher.component';
import { ChatEditCheckerComponent } from './chat-edit-checker/chat-edit-checker.component';
import { ChatAutoChecksComponent } from './chat-auto-checks/chat-auto-checks.component';
import { Exercise } from 'src/Modules/Teacher/exercise';
import { NewExe } from 'src/Modules/Teacher/new-exe';
import { HelpFile } from 'src/Modules/Teacher/help-file';
import { FileSubmit } from 'src/Modules/file-submit';
import { HttpClient } from '@angular/common/http';
import { ExOutput } from 'src/Modules/Teacher/ex-output';
import { TestInput } from 'src/Modules/Teacher/test-input';

@Component({
  selector: 'app-exe',
  templateUrl: './exe.component.html',
  styleUrls: ['./exe.component.css']
})
export class ExeComponent implements OnInit {
  modalRef: any;
  newTest: TestInput;
  theacherStatus: string = "";
  exeObject = {} as NewExe;
  editedExe: Exercise;
  uploadFileName = "";
  uploadFile: any;
  isIDcreated: boolean = false;
  teacherStatus: string;

  nameValidText: string;

  autoTest: ChatAutoChecksComponent;

  constructor(
    private appService: ApprovalService,
    public dialog: MatDialog, 
    private formBuilder: FormBuilder,
    private httpClient: HttpClient
  ) {
    this.appService.newAutoTestStorage.subscribe(test => this.newTest = test);
    this.appService.theacherStatusStorage.subscribe(status => this.teacherStatus = status);
   }

  @Input() selectedCourse: Course;
  @Input() selectExelabel: any;
  @Input() exeID: string;
  @Input() exeName: string;

  ngOnInit() {
    this.exeInit();
    console.log(this.selectedCourse);
  }

  exeInit() {
    this.exeObject.Exercise = {} as Exercise;
      
    this.exeObject.MainDate = new Date;

    this.exeObject.Exercise.name = "";
    this.exeObject.Exercise.programmingLanguage = "";
    this.exeObject.Exercise.autoTestGradeWeight = 0;
    this.exeObject.Exercise.styleTestGradeWeight = 0;
    this.exeObject.Exercise.isActive = 1;
    this.exeObject.Exercise.maxSubmitters = 3;
    this.exeObject.Exercise.multipleSubmission = false;
    this.exeObject.Exercise.reductions = [10, 20];

    this.exeObject.HelpFiles = [];

    if(this.teacherStatus === "edit") {
      this.getEditExe();
    }
  }

  getEditExe() {
    let url = 'https://localhost:5001/Teacher/ExerciseDetails?exerciseId=' + this.exeID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeObject.Exercise = JSON.parse(data);
        console.log(this.exeObject.Exercise);
      }, error => {
        console.log(error);
      }
    );
  }

  onFileChoose(event) {
    const file = event.target.files[0];
    this.uploadFile = file;
    this.uploadFileName = this.uploadFile.name;

    const reader = new FileReader();
    reader.onload = ()=> {
      //let fileBefore = reader.result.toString();
      let submitFile: HelpFile = {
        "Name": file.name,
        "Content": reader.result.toString()
      }
      this.exeObject.HelpFiles.push(submitFile);
    };
    reader.readAsDataURL(file);
  }

  openAutoChecks() {
    const modalRef = this.dialog.open(ChatAutoChecksComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
    modalRef.componentInstance.exeID = this.exeID;
  }

  openEditChekers() {
    const modalRef = this.dialog.open(ChatEditCheckerComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
    modalRef.componentInstance.selectExeId = this.exeID;
  }

  openEditTeacher() {
    const modalRef = this.dialog.open(ChatEditTeacherComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  openReduceModal() {
    const modalRef = this.dialog.open(ChatReduceComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
    modalRef.componentInstance.selectExeId = this.exeID;
    modalRef.componentInstance.exeObject = this.exeObject;
  }

  reduceTeams() {
    const modalRef = this.dialog.open(ChatRecudeTeamsComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  getAutoChecks() {
    this.theacherStatus = "autoCheck";
  }

  creat() {
    //if(!this.inputValid()) { return; }

    console.log(this.exeObject);
    let params = {
      "Exercise": this.exeObject.Exercise,
      "MainDate": this.exeObject.MainDate,
      "HelpFiles": this.exeObject.HelpFiles
    }
    let url = 'https://localhost:5001/Teacher/CreateExercise?courseid=' + this.selectedCourse.id;
    this.httpClient.post(url, params,
    {responseType: 'text'}).toPromise().then(
      data => {
        let output: ExOutput = JSON.parse(data.toString());
        this.isIDcreated = true;
        this.exeID = output.id;
        console.log(output.id);

        // this.newTest.test.exerciseID = this.exeID;
        // this.autoTest.creatTest(this.newTest);
      }, error => {
        console.log(error);
      }
    );

    if(!this.listNfilesValid()) { return; }
  }

  edit() {
    console.log(this.exeObject.Exercise);
    let params = {
      "Exercise": this.exeObject.Exercise,
      "MainDate": this.exeObject.MainDate,
      "HelpFiles": this.exeObject.HelpFiles
    }
    let url = 'https://localhost:5001/Teacher/UpdateExercise';
    this.httpClient.put(url, this.exeObject.Exercise,
    {responseType: 'text'}).toPromise().then(
      data => {
        //console.log(data.toString());
      }, error => {
        console.log(error);
      }
    );

    if(!this.listNfilesValid()) { return; }
  }
  
  inputValid(): boolean {
    if(this.exeObject.Exercise.name.length < 4) {
      return false;
    }
    return true;
  }

  listNfilesValid(): boolean {
    return true;
  }
}