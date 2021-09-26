import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { GenerealDetails } from 'src/Modules/AutoChecks/genereal-details';
import { MistakeTable } from 'src/Modules/AutoChecks/mistake-table';
import { PointsTable } from 'src/Modules/Checker/points-table';
import { TeacherDateDisplay } from 'src/Modules/Reduce/teacher-date-display';

@Component({
  selector: 'app-exe-detail',
  templateUrl: './exe-detail.component.html',
  styleUrls: ['./exe-detail.component.css']
})
export class ExeDetailComponent implements OnInit {
  uploadFileList: any[] = [];
  submitionColumns: string[] = ["typeGrade", "points", "presentage"];
  reduceTable: TeacherDateDisplay[]; //= [
  //   {
  //     name: "01",
  //     date: new Date()
  //   }, {
  //     name: "02",
  //     date: new Date()
  //   }, {
  //     name: "03",
  //     date: new Date()
  //   }
  // ];
  pointsCategorizeTable: PointsTable[] = [{
    name: "בדיקה ידנית",
    point: 94,
    weight: 30
  },
  {
    name: "בדיקה אוטו'",
    point: 90,
    weight: 60
  },
  {
    name: "בדיקת סטייל",
    point: 100,
    weight: 10
  }];
  detailColumns = ["name", "id", "rate"];
  genrealDetailsTable: GenerealDetails[] = [{
      name: "ישראל לוי",
      id: 123456789,
      rate: 98
    }
  ];

  mistakeColumns = ["mistakeType", "points"];
  mistakeCategorizeTable: MistakeTable[] = [{
    name: "הודעות מיותרות",
    points: 0
  },{
    name: "חלוקה ב0",
    points: 0
  },
  {
    name: "בדיקת סטייל",
    points: 0
  }];
  
  @Input() selectExe: any;
  @ViewChild('fileUploadBox', {static: false}) fileUploadBox: ElementRef;
  @ViewChild('fileUploadText', {static: false}) fileUploadText: ElementRef;

  constructor() { }

  ngOnInit() {

  }

  onDragOver(event) {
    this.fileUploadBox.nativeElement.style.borderStyle = "solid";
  }

  onDragLeave(event) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
  }

  onDrop(event) {
    event.preventDefault();
    this.fileUploadBox.nativeElement.style.borderStyle = "dotted";
    console.log("drop");
    let file = event.dataTransfer.files[0];

    let fileEle = document.createElement("span");
    fileEle.id = file.name;
    fileEle.innerHTML = file.name;

    this.fileUploadText.nativeElement.appendChild(fileEle);
  }

  onFileChoose(event) {
    console.log(event.target.files[0].name);
    /*if(this.exeStatus === "נבדק") {
      
    } else {
      const file = event.target.files[0];
      this.uploadFileList.push(file);
      this.isSubmitCheckFunc();
      this.file1.nativeElement.value = "";
    }

    const state = this.selectedExeInfo.state;
    if(state > 0) {
      
    }*/
  }

  eraseUploadFile(fileToDelete) {
    console.log("earse");
    const index = this.uploadFileList.indexOf(fileToDelete);
    if(index > -1) {
      this.uploadFileList.splice(index, 1);
    }
    //this.isSubmitCheckFunc();
  }

  /*isSubmitCheckFunc() {
    if(this.uploadFileList.length === 0 || this.additionalSubmitors.nativeElement.value === "") {
      this.isSubmitCheck = true;
    } else if(this.uploadFileList.length !== 0 && this.additionalSubmitors.nativeElement.value !== ""){
      this.isSubmitCheck = false;
    }
  }*/

  downloadFile(file: string) {
    this.getFilebyName(file);
  }

  getFilebyName(file: string) {
    /*let submissionID = this.selectedExeInfo.submissionID;
    let url = 'https://localhost:5001/Student/GetFile?userid=' + this.token + '&submissionId=' + submissionID + "&file=" + file;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {     
        data = data.toString();
        this.downloadAttributes(file, data);
      }, error => {
        this.errorMessage(error.status + "   try again", "alert-danger");
      }
    );*/
  }
}
