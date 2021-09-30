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
  checkersNamesList: UserLabel[];
  checkerId: string;
  token: string;

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
  ) {
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  displayedColumns: string[] = ["name", "delete"];

  ngOnInit() {
    this.getCheckers();
  }

  onSubmit() {
    this.addChecker(this.checkerId);
  }

  getCheckers() {
    console.log(this.selectedCourse);
    if(this.selectExeId === "") { return; }
    let url = 'https://localhost:5001/Teacher/GetCheckers?courseid=' + this.selectedCourse.id;
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

  addChecker(newID: string) {
    let url = 'https://localhost:5001/Teacher/AddExChecker?exercise=' + this.selectExeId + '&checkerId=' + newID;
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

  deleteChecker(id: string) {
    let url = 'https://localhost:5001/Teacher/RemoveExChecker?exercise=' + this.selectExeId + '&checkerId=' + id;
    this.httpClient.post(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        data = JSON.parse(data.toString());
        console.log(data);
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
