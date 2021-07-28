import { HttpClient } from '@angular/common/http';
import { ElementRef } from '@angular/core';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatCheckbox } from '@angular/material';
import { ApprovalService } from 'src/app/approval.service';
import { AutoCheckTable } from 'src/Modules/AutoChecks/auto-check-table';

@Component({
  selector: 'app-auto-check',
  templateUrl: './auto-check.component.html',
  styleUrls: ['./auto-check.component.css']
})
export class AutoCheckComponent implements OnInit {
  token: string;
  exeId: string;
  isAllChecked: boolean;

  dataTable: AutoCheckTable[] = [
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 123456789,
      studentName: "Yossi",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    },
    {
      id: 789456123,
      studentName: "Michal",
      statusCheck: "טרם",
      statusSubmission: "נשלח",
      autoGrade: -1,
      manualGrade: -1,
      checked: false
    }
  ];


  displayedColumns: string[] = ['name', 'checkStatus', 'submissionStatus', 'autoCheck', 'manualCheck', 'mark'];
  dataSource = this.dataTable;

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  @Input() selectExelabel: any;

  ngOnInit() {
    this.getExeID();
    this.getSubmissionToCheck();
  }

  getExeID() {
    let url = 'https://localhost:5001/Student/SubmissionListToCheck?userid=' + this.token + '&exerciseId=';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.dataTable = JSON.parse(data);
        console.log(this.dataTable);
      }, error => {
        console.log(error);
      }
    );
  }

  getSubmissionToCheck() {
    let url = 'https://localhost:5001/Student/SubmissionListToCheck?userid=' + this.token + '&exerciseId=';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.dataTable = JSON.parse(data);
        console.log(this.dataTable);
      }, error => {
        console.log(error);
      }
    );
  }

  selectAll() {
    console.log(this.isAllChecked);
    if(!this.isAllChecked) {
      this.dataTable.forEach(element => {
        element.checked = false;
      });
    }
  }

  startCheck() {
    let flag: boolean = false;
    this.dataTable.forEach(element => {
      if(element.checked) {
        flag = true;
      }
    });

    if(!flag && !this.isAllChecked) {
      // Alert
      return;
    }

    if(confirm("Start Auto check?")) {
      console.log("Start check");
      // TODO
    }
  }
}
