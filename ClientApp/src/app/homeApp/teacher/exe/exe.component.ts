import { Component, Input, OnInit } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { Course } from 'src/Modules/course';
import { ChatReduceComponent } from './chat-reduce/chat-reduce.component';
import { ChatRecudeTeamsComponent } from './chat-recude-teams/chat-recude-teams.component';
import { ChatEditTeacherComponent } from './chat-edit-teacher/chat-edit-teacher.component';
import { ChatEditCheckerComponent } from './chat-edit-checker/chat-edit-checker.component';
import { ChatAutoChecksComponent } from './chat-auto-checks/chat-auto-checks.component';

@Component({
  selector: 'app-exe',
  templateUrl: './exe.component.html',
  styleUrls: ['./exe.component.css']
})
export class ExeComponent implements OnInit {
  modalRef: any;
  theacherStatus: string = "";

  constructor(
    private appService: ApprovalService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  @Input() selectedCourse: Course;
  @Input() selectExelabel: any;

  ngOnInit() {
    console.log(this.selectedCourse);
  }
  openAutoChecks() {
    const modalRef = this.dialog.open(ChatAutoChecksComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  openEditChekers() {
    const modalRef = this.dialog.open(ChatEditCheckerComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  openEditTeacher() {
    const modalRef = this.dialog.open(ChatEditTeacherComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  openReduceModal() {
    const modalRef = this.dialog.open(ChatReduceComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  reduceTeams() {
    const modalRef = this.dialog.open(ChatRecudeTeamsComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
  }

  getAutoChecks() {
    this.theacherStatus = "autoCheck";
  }
}
