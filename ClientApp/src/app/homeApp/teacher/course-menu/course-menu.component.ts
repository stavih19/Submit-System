import { HttpClient } from '@angular/common/http';
import { ElementRef, EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';

@Component({
  selector: 'app-course-menu',
  templateUrl: './course-menu.component.html',
  styleUrls: ['./course-menu.component.css']
})
export class CourseMenuComponent implements OnInit {
  token: string;
  allExeList: any;
  exeNameList: any[];
  teacherNamesList: string[];
  checkersNamesList: string[];

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) {
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    this.getExercises();
    this.getTeachers();
    this.getCheckers();
  }

  @Input() selectedCourse: Course;
  @ViewChild('teacherID', { static: false}) teacherID: ElementRef;
  @ViewChild('checkerID', { static: false}) checkerID: ElementRef;
  @Output() exeID = new EventEmitter<string>();
  @Output() exeName = new EventEmitter<string>();
  @Output() isToShowAlert = new EventEmitter<boolean>();
  @Output() color = new EventEmitter<string>();
  @Output() errorMessageText = new EventEmitter<string>();

  getExercises() {
    let url = 'https://localhost:5001/Teacher/AllExercises?token=' + this.token + '&courseid=' + this.selectedCourse.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.allExeList = JSON.parse(data);
        this.exeNameList = this.allExeList[(new Date()).getFullYear().toString()];
        console.log(this.exeNameList);
      }, error => {
        console.log(error);
      }
    );
  }

  getTeachers() {
    let url = 'https://localhost:5001/Teacher/GetTeachers?token=' + this.token + '&courseid=' + this.selectedCourse.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.teacherNamesList = JSON.parse(data);
        console.log(this.teacherNamesList);
      }, error => {
        console.log(error);
      }
    );
  }

  getCheckers() {
    let url = 'https://localhost:5001/Teacher/GetTeachers?token=' + this.token + '&courseid=' + this.selectedCourse.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.checkersNamesList = JSON.parse(data);
        console.log(this.checkersNamesList);
      }, error => {
        console.log(error);
      }
    );
  }

  createExe() {
    this.appService.updateTheacherStatus("create");
  }

  reCreateExe() {
    // Change UI page; TODO
    // same as cerate
  }

  editExe(index: number) {
    this.exeID.emit(this.exeNameList[index].id);
    this.exeName.emit(this.exeNameList[index].name)
    this.appService.updateTheacherStatus("edit");
  }

  addTeacher() {
    const newID = this.teacherID.nativeElement.value;
    if(this.checkIDvalidation(newID)) { return; }
    
    let params = {
      "courseid": this.selectedCourse.id,
      "teacherid": newID,
    };
    console.log(params);
    let url = 'https://localhost:5001/Teacher/AddTeacher?courseid=' + this.selectedCourse.id + '&teacherid=' + newID;
    console.log(this.selectedCourse);
    this.httpClient.post(url, params, 
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data.toString());
        this.getTeachers();
        console.log(data);
      }, error => {
        console.log(error);
      }
    );
  }

  addChecker() {
    const newID = this.checkerID.nativeElement.value;
    console.log(newID);
    if(this.checkIDvalidation(newID)) { return; }

    let url = 'https://localhost:5001/Teacher/AddChecker?courseid=' + this.selectedCourse.id + '&checkerid=' + newID;
    this.httpClient.post(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data.toString());
        this.getCheckers();
        console.log(data);
      }, error => {
        console.log(error);
      }
    );
  }

  checkIDvalidation(id: any): boolean {
    if(id.length < 9) {
      this.errorMessage("alert-danger", "Short ID number");
      return true;
    }
    if(id.length > 9) {
      this.errorMessage("alert-danger", "Long ID number");
      return true;
    }
    console.log(parseInt(id));
    const idNum = parseInt(id);
    if(isNaN(idNum) || String(idNum).length < 9 || String(idNum).length > 9) {
      this.errorMessage("alert-danger", "Not number");
      return true;
    }
  }
  
  getTeacherName(id: string): string{
    return id;
  }
  
  getCheckerName(id: string): string{
    return id;
  }

  deleteExe(index: number) {
    let url = 'https://localhost:5001/Teacher/DeleteExercise?exerciseId=' + this.exeNameList[index].id;
    this.httpClient.delete(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data.toString());
        setTimeout(() => {
          this.getCheckers();
        }, 1000);
      }, error => {
        console.log(error);
      }
    );
  }

  deleteTeacher(index: number) {
    //console.log(this.teacherNamesList.splice(index, 1));
  }

  deleteChecker(index: number) {
    //console.log(this.checkersNamesList.splice(index, 1));
  }

  errorMessage(color:string, message: string) {
    this.updateShowAlert(true, color, message);
    setTimeout(() => {
      this.updateShowAlert(false, color, message);
    }, 5000);
  }

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
}