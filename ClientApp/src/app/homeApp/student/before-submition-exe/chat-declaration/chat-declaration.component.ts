import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chat-declaration',
  templateUrl: './chat-declaration.component.html',
  styleUrls: ['./chat-declaration.component.css']
})
export class ChatDeclarationComponent implements OnInit {
  data: string = "<a>click</a>";

  constructor() { }

  ngOnInit() {
    
  }
}
