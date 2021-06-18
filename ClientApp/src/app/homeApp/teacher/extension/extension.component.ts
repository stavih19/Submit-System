import { Component, OnInit } from '@angular/core';
import { ExtenstionRequest } from 'src/Modules/Extenstion/extenstion-request';
import { TeamLabel } from 'src/Modules/Reduce/team-label';

@Component({
  selector: 'app-extension',
  templateUrl: './extension.component.html',
  styleUrls: ['./extension.component.css']
})
export class ExtensionComponent implements OnInit {

  requests: ExtenstionRequest[] = [
    {
      name: "יוסי",
      id: 123456789,
      text: "שלום מתרגל",
      file: "file.pdf",
      team: "01"
    },
    {
      name: "רמי",
      id: 123456789,
      text: "שלום מתרגל יקר",
      file: "file.pdf",
      team: "01"
    }
  ];

  teams: string[] = ["01", "02", "03"];
  selectedOption: string = "01";

  reduceTable: TeamLabel[] = [
    {
      name: "01",
      date: new Date()
    }, {
      name: "02",
      date: new Date()
    }, {
      name: "03",
      date: new Date()
    }
  ];
  submitionColumns: string[] = ["teamDate", "teamName"];

  constructor() { }

  ngOnInit() {
  }

}
