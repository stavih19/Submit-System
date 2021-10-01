import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { CheckerLabel } from 'src/Modules/Reduce/checker-label';
import { UserLabel } from 'src/Modules/Teacher/user-label';

@Component({
  selector: 'app-chat-edit-checker',
  templateUrl: './chat-edit-checker.component.html',
  styleUrls: ['./chat-edit-checker.component.css']
})
export class ChatEditCheckerComponent implements OnInit {
  selectedCourse: Course;
  selectExeId: string;
  allCheckersNamesList: UserLabel[];
  checkersNamesList:  UserLabel[];
  checkerId: string;
  token: string;

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
  ) {
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  displayedColumns: string[] = ["delete", "name"];

  ngOnInit() {
    this.getCheckers();
  }

  onSubmit() {
    this.addChecker(this.checkerId);
  }

  getAllCheckers() {
    console.log(this.selectedCourse);
    if(this.selectExeId === "") { return; }
    let url = 'https://localhost:5001/Teacher/GetCheckers?courseid=' + this.selectedCourse.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.allCheckersNamesList = [];
        let allCheckersNamesListTemp: UserLabel[] = JSON.parse(data);
        let isExist: boolean = false;
        allCheckersNamesListTemp.forEach(maybeRelateChecker => {
          this.checkersNamesList.forEach(relateChecker => {
            if(maybeRelateChecker.id === relateChecker.id) {
              isExist = true;
            }
          });
          if(!isExist) {
            this.allCheckersNamesList.push(maybeRelateChecker);
          }
          isExist = false;
        });
        console.log(this.allCheckersNamesList);
      }, error => {
        console.log(error);
      }
    );
  }

  getCheckers() {
    if(this.selectExeId === "") { return; }
    let url = 'https://localhost:5001/Teacher/ExerciseCheckers?exerciseId=' + this.selectExeId;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.checkersNamesList = JSON.parse(data);
        console.log(this.checkersNamesList);
        this.getAllCheckers();
      }, error => {
        console.log(error);
      }
    );
  }

  addChecker(newID: string) {
    let url = 'https://localhost:5001/Teacher/AddExerciseChecker?exerciseId=' + this.selectExeId + '&checkerId=' + newID;
    this.httpClient.post(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        this.getCheckers();
        console.log(data);
      }, error => {
        console.log(error);
      }
    );
  }

  deleteChecker(id: string) {
    console.log(id);
    let url = 'https://localhost:5001/Teacher/RemoveExerciseChecker?exerciseId=' + this.selectExeId + '&checkerid=' + id;
    this.httpClient.delete(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data);
        this.getCheckers();
      }, error => {
        console.log(error);
      }
    );
  }

  changeClient(id: string) {
    this.checkerId = id;
    console.log(id);
  }
}
