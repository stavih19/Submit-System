import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { GradeTable } from 'src/Modules/grade-table';
import { RequestLabelMainPage } from 'src/Modules/Teacher/request-label-table';
import { SubmitTable } from 'src/Modules/Teacher/submit-table';

@Component({
  selector: 'app-teacher',
  templateUrl: './teacher.component.html',
  styleUrls: ['./teacher.component.css']
})
export class TeacherComponent implements OnInit {
  exeStatus: string;
  theacherStatus: string;
  selectedCourse: Course;
  selectExe: RequestLabelMainPage;
  selectExelabel: any;
  headerText: string;
  teacherName: string;
  coursesList: Course[];
  lastExeColumns: string[];
  lastExeDataSource: SubmitTable[];
  extenstionDataSource: RequestLabelMainPage[];
  appealDataSource: RequestLabelMainPage[];
  gradeColumns: any;
  requestColumns: string[];
  gradeDataSource: GradeTable[];
  token: string;
  isToShowAlert: boolean;
  color: string;
  errorMessage: string;
  exeID: string;
  exeName: string;

  @ViewChild("alert", {static: false}) alert: ElementRef;

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.appService.theacherStatusStorage.subscribe(theacherStatus => this.theacherStatus = theacherStatus);
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    this.appService.updateExeStatus("");

    this.getCourseList();
    this.getAppealsLoad();
    this.submitionLoad();
    this.extenstionLoad();
  }

  onMark(course: Course): void {
    this.selectedCourse = course;
  }

  onSelect(course: Course): void {
    this.selectedCourse = course;
    this.appService.updateTheacherStatus("course");
  }

  getAppeals(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.headerText = "בקשות ערעור";
    this.appService.updateTheacherStatus("appeal");
  }

  getExtenstions(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.headerText = "בקשות הארכה";
    this.appService.updateTheacherStatus("extenstion");
  }

  getLastExe(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.coursesList.forEach(course => {
      if(course.name === row.courseName) {
        this.selectedCourse = course;
        this.selectExelabel = row;
        this.appService.updateTheacherStatus("lastExe");
      }
    });
  }

  getExeInfo() { }

  getCourseList() {
    let url = 'https://localhost:5001/Teacher/CourseList';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
        console.log(this.coursesList);
      }, error => {
        console.log(error);
      }
    );
  }

  getAppealsLoad() {
    this.requestColumns = ["courseName", "courseNumber", "exeName", "studentName"];
    let url = 'https://localhost:5001/Teacher/GetTeacherAppeals';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.appealDataSource = JSON.parse(data);
        console.log(this.appealDataSource);
      }, error => {
        console.log(error);
      }
    );
  }

  submitionLoad() {
    this.lastExeColumns = ['courseName', 'courseNumber', 'exeName'];//, 'teacherName'];
    let url = 'https://localhost:5001/Teacher/ExerciseList';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.lastExeDataSource = JSON.parse(data);
        console.log(this.lastExeDataSource);
      }, error => {
        console.log(error);
      }
    );
  }

  extenstionLoad() {
    let url = 'https://localhost:5001/Teacher/GetTeacherExtensions';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.extenstionDataSource = JSON.parse(data);
        console.log(this.extenstionDataSource);
      }, error => {
        console.log(error);
      }
    );
  }

  changeIsToShowAlert(isToShowAlert: boolean) {
    this.isToShowAlert = isToShowAlert;
  }

  changeColorAlert(color: string) {
    this.color = color;
    
    this.alert.nativeElement.classList.remove("alert-success");
    this.alert.nativeElement.classList.remove("alert-danger");
    this.alert.nativeElement.classList.remove("alert-warning");
    this.alert.nativeElement.classList.add(color);
  }

  changeMessageAlert(errorMessage: string) {
    console.log(errorMessage);
    this.errorMessage = errorMessage;
  }

  changeExeID(newExeId: string) {
    this.exeID = newExeId;
  }

  changeExeName(exeName: string) {
    this.exeName = exeName;
  }
}