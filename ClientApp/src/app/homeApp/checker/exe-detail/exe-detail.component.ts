import { Component, Input, OnInit } from '@angular/core';
import { GenerealDetails } from 'src/Modules/AutoChecks/genereal-details';
import { PointsTable } from 'src/Modules/Checker/points-table';
import { TeamLabel } from 'src/Modules/Reduce/team-label';

@Component({
  selector: 'app-exe-detail',
  templateUrl: './exe-detail.component.html',
  styleUrls: ['./exe-detail.component.css']
})
export class ExeDetailComponent implements OnInit {
  submitionColumns: string[] = ["typeGrade", "points", "presentage"];
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
  detailColumns = ["name", "id"];
  genrealDetailsTable: GenerealDetails[] = [{
      name: "ישראל לוי",
      id: 123456789
    }
  ];

  @Input() selectExe: any;

  constructor() { }

  ngOnInit() {

  }
}
