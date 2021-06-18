import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatEditTeacherComponent } from './chat-edit-teacher.component';

describe('ChatEditTeacherComponent', () => {
  let component: ChatEditTeacherComponent;
  let fixture: ComponentFixture<ChatEditTeacherComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatEditTeacherComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatEditTeacherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
