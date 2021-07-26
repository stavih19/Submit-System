import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatRecudeTeamsComponent } from './chat-recude-teams.component';

describe('ChatRecudeTeamsComponent', () => {
  let component: ChatRecudeTeamsComponent;
  let fixture: ComponentFixture<ChatRecudeTeamsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatRecudeTeamsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatRecudeTeamsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
