import { ElementRef, EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-course-menu',
  templateUrl: './course-menu.component.html',
  styleUrls: ['./course-menu.component.css']
})
export class CourseMenuComponent implements OnInit {
  exeNameList: string[] = ["exe 1", "exe 2", "exe 3", "exe 4"];
  oldExeList: any[] = [
    {
      year: "2020",
      exeNameList: ["exe 1", "exe 2"]
    },
    {
      year: "2021",
      exeNameList: ["exe 1", "exe 2", "exe 3"]
    }
  ];
  teacherNamesList: string[] = ["חגי", "יובל", "שני"];
  checkersNamesList: string[] = ["יוסי", "עדי", "אביעד"];

  constructor(
    private appService: ApprovalService,
  ) { }

  ngOnInit() {
    
  }

  @Input() selectedCourse: any;
  @ViewChild('teacherID', { static: false}) teacherID: ElementRef;
  @ViewChild('checkerID', { static: false}) checkerID: ElementRef;
  @Output() isToShowAlert = new EventEmitter<boolean>();
  @Output() color = new EventEmitter<string>();
  @Output() errorMessageText = new EventEmitter<string>();

  createExe() {
    this.appService.updateTheacherStatus("create");
  }

  reCreateExe() {
    // Change UI page; TODO
    // same as cerate
  }

  editExe(index) {
    this.appService.updateTheacherStatus("edit");
  }

  addTeacher() {
    const newID = this.teacherID.nativeElement.value;
    if(this.checkIDvalidation(newID)) { return; }
    const teacherName = this.getTeacherName(newID);
    this.teacherNamesList.push(teacherName);
    this.teacherID.nativeElement.value = "";
  }

  addChecker() {
    const newID = this.checkerID.nativeElement.value;
    if(this.checkIDvalidation(newID)) { return; }
    const teacherName = this.getCheckerName(newID);
    this.checkersNamesList.push(teacherName);
    this.checkerID.nativeElement.value = "";
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

  deleteExe(index) {
    console.log(this.exeNameList.splice(index, 1));
  }

  deleteTeacher(index) {
    console.log(this.teacherNamesList.splice(index, 1));
  }

  deleteChecker(index) {
    console.log(this.checkersNamesList.splice(index, 1));
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