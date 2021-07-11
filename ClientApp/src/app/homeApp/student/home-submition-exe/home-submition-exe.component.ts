import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExerciseLabel } from 'src/Modules/exercise-label';
import { BeforeSubmitionExeComponent } from '../before-submition-exe/before-submition-exe.component';

@Component({
  selector: 'app-home-submition-exe',
  templateUrl: './home-submition-exe.component.html',
  styleUrls: ['./home-submition-exe.component.css']
})

export class HomeSubmitionExeComponent implements OnInit {
  token: string;
  exeStatus: string;
  isToShowAlert: boolean;
  color: string;
  errorMessage: string;
  exeList: ExerciseLabel[];
  selectedExe: ExerciseLabel;

  @ViewChild(BeforeSubmitionExeComponent, {static: false}) child: BeforeSubmitionExeComponent;
  @ViewChild("alert", {static: false}) alert: ElementRef;
  @Input() selectExe: any;
  @Input() teacherName: any;

  constructor(
    private httpClient: HttpClient,
    private appService: ApprovalService,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    this.getExeLists(this.selectExe.courseID);
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

  getExeLists(courseID) {
    let url = 'https://localhost:5001/Student/ExerciseList?userid=' + this.token + '&coursed=' + courseID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeList = JSON.parse(data);
        this.onSelect(this.exeList[0]); // TODO
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

  onSelect(exe) {
    this.selectedExe = exe;
    this.child.onSelect(exe);
  }
}